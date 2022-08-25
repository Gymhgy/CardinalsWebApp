using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

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

                ProcessSchedule(store);
            }
            catch (ArgumentException e) {
                Console.WriteLine(e.Message);
                return false;
            }

            if (store) {
                Console.WriteLine(user.Username + " " + user.Password);
                _ = js.InvokeVoidAsync("window.WriteCookie", "username", user.Username, 30);
                _ = js.InvokeVoidAsync("window.WriteCookie", "password", user.Password, 30);
            }

            return true;
        }

        public async Task Logout() {
            User = null;
            Gradebook = null;
            ClassNames.Clear();
            _ = js.InvokeVoidAsync("window.DeleteCookie", "username");
            _ = js.InvokeVoidAsync("window.DeleteCookie", "password");
            _ = js.InvokeVoidAsync("window.DeleteCookie", "schedule");

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
                ClassNames = schedule.Split("|").ToDictionary(item => item.Split("+")[0], item => item.Split("+")[1]);
            }
            catch { }
        }

        public string GetName(string period) {
            if (ClassNames.TryGetValue(period, out string name)) {
                return name;
            }
            else return "";
        }

        static Dictionary<string, string> fourthAB = new Dictionary<string, string> {
            ["M.+ Ambrose"] = "A",
            ["A.+ Amirapu"] = "B",
            ["K.+ Arnold"] = "B",
            ["L.+ Bajet"] = "A",
            ["J.+ Brooks"] = "A",
            ["S.+ Carney"] = "A",
            ["T.+ Centers"] = "A",
            ["Ja.+ Chan"] = "B",
            ["Je.+ Chan"] = "A",
            ["R.+ Chan"] = "A",
            ["K.+ Chassagne"] = "A",
            ["A.+ Cheng"] = "B",
            ["A.+ Cho"] = "B",
            ["C.+ Chu"] = "B",
            ["L.+ Cole"] = "A",
            ["A.+ Cowan-Byrns"] = "B",
            ["S.+ Crabtree"] = "A",
            ["S.+ De La Riarte"] = "B",
            ["S.+ Dickerman"] = "B",
            ["T.+ Doherty"] = "B",
            ["C.+ Ferrey"] = "B",
            ["J.+ Fong"] = "B",
            ["S.+ Fortin"] = "A",
            ["K.+ Franklin"] = "B",
            ["M.+ Furey"] = "B",
            ["L.+ Galang-McMahon"] = "A",
            ["C.+ Gaver"] = "A",
            ["T.+ Geren"] = "A",
            ["C.+ Gersten"] = "B",
            ["J.+ Gribler"] = "B",
            ["L.+ Grice"] = "B",
            ["E.+ Gustafson"] = "A",
            ["E.+ Hanlon-Young"] = "A",
            ["S.+ Hardee"] = "B",
            ["K.+ Hoffman"] = "A",
            ["C.+ Hosoda"] = "B",
            ["R.+ Johnson"] = "B",
            ["W.+ Jones"] = "B",
            ["A.+ Kent"] = "B",
            ["A.+ Killpack"] = "A",
            ["D.+ Kim"] = "A",
            ["T.+ Kiramichyan"] = "B",
            ["A.+ Klein"] = "B",
            ["I.+ Knight"] = "A",
            ["L.+ Komlos"] = "B",
            ["S.+ Laureyns"] = "B",
            ["Y.+ Li"] = "B",
            ["J.+ Liang"] = "B",
            ["K.+ Liu"] = "A",
            ["J.+ Lopez"] = "B",
            ["K.+ Lubenow"] = "B",
            ["A.+ Maldonado-Silva"] = "A",
            ["B.+ Marten"] = "B",
            ["M.+ Martinez"] = "A",
            ["K.+ Melvin"] = "B",
            ["A.+ Michels"] = "A",
            ["H.+ Milstein"] = "B",
            ["J.+ Moffitt"] = "A",
            ["E.+ Moore"] = "A",
            ["J.+ Moses"] = "B",
            ["N.+ Okada"] = "A",
            ["L.+ Pang"] = "A",
            ["K.+ Petrini"] = "A",
            ["G.+ Plastina"] = "A",
            ["J.+ Raya"] = "B",
            ["E.+ Reller"] = "B",
            ["B.+ Ritter"] = "A",
            ["F.+ Robinson"] = "B",
            ["C.+ Samayoa"] = "B",
            ["L.+ Sandzik-Robinson"] = "A",
            ["M.+ Santiago"] = "B",
            ["W.+ Sinn"] = "B",
            ["Staffe"] = "B",
            ["S.+ Staffn"] = "B",
            ["E.+ Statmore"] = "B",
            ["K.+ Sullivan"] = "A",
            ["S.+ Taylor-Ray"] = "A",
            ["A.+ Torres"] = "B",
            ["M.+ Trimble"] = "A",
            ["J.+ Tuason"] = "A",
            ["M.+ Ungar"] = "B",
            ["C.+ Villanueva"] = "B",
            ["M.+ Wagner"] = "B",
            ["J.+ Welch"] = "A",
            ["S.+ Wong"] = "A"
        };
        static Dictionary<string, string> thirdAB = new Dictionary<string, string> {
            ["A.+ Amirapu"] = "B",
            ["K.+ Arnold"] = "A",
            ["L.+ Bajet"] = "A",
            ["S.+ Bookwalter"] = "B",
            ["J.+ Brooks"] = "A",
            ["C.+ Cadoppi"] = "A",
            ["Ja.+ Chan"] = "A",
            ["Je.+ Chan"] = "A",
            ["R.+ Chan"] = "A",
            ["A.+ Cheng"] = "B",
            ["A.+ Cho"] = "B",
            ["C.+ Chu"] = "B",
            ["B.+ Cooley"] = "B",
            ["A.+ Cowan-Byrns"] = "B",
            ["S.+ Crabtree"] = "A",
            ["B.+ Danforth"] = "B",
            ["S.+ De La Riarte"] = "B",
            ["S.+ Dickerman"] = "B",
            ["T.+ Doherty"] = "A",
            ["M.+ Durham"] = "A",
            ["J.+ Fong"] = "B",
            ["S.+ Fortin"] = "B",
            ["K.+ Franklin"] = "B",
            ["M.+ Furey"] = "B",
            ["C.+ Gaver"] = "A",
            ["T.+ Geren"] = "B",
            ["C.+ Gersten"] = "B",
            ["J.+ Gribler"] = "A",
            ["L.+ Grice"] = "B",
            ["E.+ Gustafson"] = "A",
            ["E.+ Hanlon-Young"] = "A",
            ["S.+ Hardee"] = "A",
            ["N.+ Henares"] = "B",
            ["L.+ Hong"] = "A",
            ["R.+ Johnson"] = "B",
            ["W.+ Jones"] = "A",
            ["C.+ Keenan"] = "A",
            ["A.+ Killpack"] = "A",
            ["T.+ Kiramichyan"] = "A",
            ["A.+ Klein"] = "A",
            ["I.+ Knight"] = "A",
            ["V.+ LaMastra"] = "A",
            ["S.+ Laureyns"] = "B",
            ["Y.+ Li"] = "A",
            ["J.+ Liang"] = "B",
            ["S.+ Lin"] = "B",
            ["K.+ Liu"] = "B",
            ["J.+ Lopez"] = "A",
            ["K.+ Lubenow"] = "A",
            ["M.+ Magsanay"] = "A",
            ["R.+ Marshall"] = "B",
            ["B.+ Marten"] = "B",
            ["M.+ Martinez"] = "B",
            ["K.+ Melvin"] = "B",
            ["C.+ Mercado"] = "A",
            ["H.+ Milstein"] = "A",
            ["K.+ Mitchell"] = "B",
            ["J.+ Moffitt"] = "A",
            ["H.+ Nghe"] = "B",
            ["N.+ Okada"] = "A",
            ["L.+ Pang"] = "A",
            ["C.+ Park"] = "A",
            ["C.+ Pelagatti"] = "A",
            ["J.+ Pollock"] = "A",
            ["R.+ Prothro"] = "B",
            ["C.+ Puretz"] = "A",
            ["J.+ Raya"] = "B",
            ["C.+ Samayoa"] = "A",
            ["L.+ Sandzik-Robinson"] = "A",
            ["M.+ Santiago"] = "B",
            ["W.+ Sinn"] = "B",
            ["S.+ Staffd"] = "B",
            ["S.+ Staffn"] = "A",
            ["S.+ Staffn"] = "A",
            ["S.+ Staffn"] = "A",
            ["S.+ Staffn"] = "A",
            ["S.+ Staffp"] = "A",
            ["T.+ Sullivan"] = "A",
            ["S.+ Taylor-Ray"] = "A",
            ["R.+ Tran"] = "A",
            ["M.+ Trimble"] = "A",
            ["M.+ Ungar"] = "B",
            ["J.+ Welch"] = "B",
            ["M.+ Wenning"] = "A",
            ["K.+ West"] = "B",
            ["S.+ Williams"] = "A",
            ["J.+ Worth"] = "A",
            ["D.+ Yoshimura"] = "B",
            ["S.+ Yu"] = "A",
            ["D.+ Yuan"] = "B",
        };
        private async void ProcessSchedule(bool store) {
            ClassNames.Clear();
            List<string> cookie = new();
            foreach(var course in Gradebook.Descendants("Course")) {
                var period = course.Attribute("Period").Value;
                var courseName = new Regex(@"\(.+").Replace(course.Attribute("Title").Value, "");
                var teacher = course.Attribute("Staff").Value;
                var room = course.Attribute("Room").Value;

                string description = teacher + "\u2014" + courseName + " Rm " + room;

                if(period == "3") {
                    if(store) {
                        cookie.Add(period + "+" + description);
                    }
                    ClassNames.Add(period, description);
                    period += thirdAB.FirstOrDefault(kv => Regex.IsMatch(teacher, kv.Key)).Value ?? "";
                }
                if (period == "4") {
                    period += fourthAB.FirstOrDefault(kv => Regex.IsMatch(teacher, kv.Key)).Value ?? "";
                }
                ClassNames.Add(period, description);
                if (store) {
                    cookie.Add(period + "+" + description);
                }
            }
            Console.Write(string.Join("|", cookie));
            if(store) {
                await js.InvokeVoidAsync("window.WriteCookie", "schedule", string.Join("|", cookie), 30);
            }
        }
	}
}
