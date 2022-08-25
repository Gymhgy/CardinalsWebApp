using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.JSInterop;

namespace CardinalsWebApp {
    public class User {
        [Required]
        public string Username { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        public string Domain { get; set; } = "portal.sfusd.edu";
    }
    public interface IAccountService {
        User? User { get; }
        XDocument? Gradebook { get; }
        Dictionary<string, string> ClassNames { get; }
        Task<bool> LoginAsync(User user, bool store);
        void InitSchedule(string schedule);
        Task Logout();

        string GetName(string period);
    }
    public class AccountService : IAccountService {
        public User? User { get; private set; }

        public XDocument? Gradebook { get; private set; }
        public Dictionary<string, string> ClassNames { get; private set; } = new Dictionary<string, string>();

        private readonly HttpClient http;
        private readonly IJSRuntime js;

        public AccountService(HttpClient http, IJSRuntime js) {
            this.http = http;
            this.js = js;
        }

        private static string err = "Source: Exception, Msg: The user name or password is incorrect.";
        public async Task<bool> LoginAsync(User user, bool store = false) {
            User = user;
            try {
                Gradebook = await SendRequestAsync(user.Username, user.Password, user.Domain, "Gradebook", "", http);

                ClassNames = Gradebook.Descendants("Course").ToDictionary(x => x.Attribute("Period").Value, x => x.Attribute("Title").Value);
            }
            catch (ArgumentException e) {
                Console.WriteLine(e.Message);
                return false;
			}

            if (store) {
                Console.WriteLine(user.Username + " " + user.Password);
                await js.InvokeVoidAsync("window.WriteCookie", "username", user.Username, 30);
                await js.InvokeVoidAsync("window.WriteCookie", "password", user.Password, 30);
                string s = string.Join(";",
                    Gradebook.Descendants("Course").Select(x => x.Attribute("Period").Value + "=" + x.Attribute("Title").Value));
                Console.WriteLine(s);
                await js.InvokeVoidAsync("window.WriteCookie", "schedule", string.Join(";",
                    Gradebook.Descendants("Course").Select(x => x.Attribute("Period").Value + "=" + x.Attribute("Title").Value)), 30);
            }

            return true;
        }

        public async Task Logout() {
            User = null;
            Gradebook = null;
            ClassNames.Clear();
            await js.InvokeVoidAsync("window.DeleteCookie", "username");
            await js.InvokeVoidAsync("window.DeleteCookie", "password");
            await js.InvokeVoidAsync("window.DeleteCookie", "schedule");

        }
        public static async Task<XDocument> SendRequestAsync(string username, string password, string domain, string method, string parms, HttpClient http) {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://svue-proxy.herokuapp.com/svue/{domain}");
            request.Headers.Add("SOAPAction", "\"http://edupoint.com/webservices/ProcessWebServiceRequest\"");
            request.Content = new StringContent(
                "<soap-env:Envelope xmlns:soap-env=\"http://schemas.xmlsoap.org/soap/envelope/\"><soap-env:Body><ns0:ProcessWebServiceRequest xmlns:ns0=\"http://edupoint.com/webservices/\">" +
                    $"<ns0:userID>{username}</ns0:userID>" +
                    $"<ns0:password>{password}</ns0:password>" +
                    "<ns0:skipLoginLog>true</ns0:skipLoginLog>" +
                    "<ns0:parent>false</ns0:parent>" +
                    "<ns0:webServiceHandleName>PXPWebServices</ns0:webServiceHandleName>" +
                    $"<ns0:methodName>{method}</ns0:methodName>" +
                    $"<ns0:paramStr>&lt;Parms&gt;{parms}&lt;/Parms&gt;</ns0:paramStr>" +
                "</ns0:ProcessWebServiceRequest></soap-env:Body></soap-env:Envelope>",
                null, "text/xml");
            var response = await (await http.SendAsync(request)).Content.ReadAsStringAsync();

            if (response.Contains(err)) throw new ArgumentException("username/password");

            var responseXml = XDocument.Parse(response);
            XNamespace xmlns = "http://edupoint.com/webservices/";
            var xml = responseXml.Descendants(xmlns + "ProcessWebServiceRequestResult").First().Value.Replace("<br>", "&#xA;");

            return XDocument.Parse(xml);

        }

		public void InitSchedule(string schedule) {
            try {
                ClassNames = schedule.Split(";").ToDictionary(item => item.Split("=")[0], item => item.Split("=")[1]);
            }
            catch { }
		}

        public string GetName(string period) {
            if (ClassNames.TryGetValue(period, out string name)) {
                return name;
            }
            else return "";
		}
	}
}
