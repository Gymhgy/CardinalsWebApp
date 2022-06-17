namespace CardinalsWebApp {
    public static class ScheduleProvider {


        static readonly DateOnly[] rallies = new DateOnly[] {
            new DateOnly(2022, 8, 19),
            new DateOnly(2022, 10, 7),
            new DateOnly(2023, 1, 3),
            new DateOnly(2023, 2, 24),
            new DateOnly(2023, 4, 15)
        };
        //Oh god why did I use TimeOnly instead of a string
        static readonly Schedule rallySchedule = new Schedule(new PeriodBase[] {
            new Period("Registry / Rally", new TimeOnly(8, 40), new TimeOnly(9, 50)),
            new Period("1", new TimeOnly(9, 55), new TimeOnly(10, 35)),
            new Period("2", new TimeOnly(10, 40), new TimeOnly(11, 20)),
            new Period("3", new TimeOnly(11, 25), new TimeOnly(12, 05)),
            new Lunch(
                new LunchSlot("A Lunch / 4", 
                    new Period("A Lunch", new TimeOnly(12, 05), new TimeOnly(12,45)), 
                    new Period("4", new TimeOnly(12,50), new TimeOnly(13, 30))),
                new LunchSlot("4 / B Lunch",
                    new Period("4", new TimeOnly(12, 10), new TimeOnly(12,50)),
                    new Period("B Lunch", new TimeOnly(12,50), new TimeOnly(13, 30)))),            new Period("5", new TimeOnly(13, 35), new TimeOnly(14, 15)),
            new Period("6", new TimeOnly(14, 20), new TimeOnly(15, 00)),
            new Period("7", new TimeOnly(15, 05), new TimeOnly(15, 45)),
        });

        static readonly DateOnly[] firstWeek = new DateOnly[] {
            new DateOnly(2022, 8, 17),
            new DateOnly(2022, 8, 18)
        };
        static readonly Schedule firstWeekSchedule = new Schedule(new PeriodBase[] {
            new Period("Registry", new TimeOnly(8, 40), new TimeOnly(9, 10)),
            new Period("1", new TimeOnly(9, 15), new TimeOnly(10, 00)),
            new Period("2", new TimeOnly(10, 05), new TimeOnly(10, 50)),
            new Period("3", new TimeOnly(10, 55), new TimeOnly(11, 40)),
            new Lunch(
                new LunchSlot("A Lunch / 4",
                    new Period("A Lunch", new TimeOnly(11, 40), new TimeOnly(12,20)),
                    new Period("4", new TimeOnly(12,25), new TimeOnly(13, 10))),
                new LunchSlot("4 / B Lunch",
                    new Period("4", new TimeOnly(11, 45), new TimeOnly(12,30)),
                    new Period("B Lunch", new TimeOnly(12,30), new TimeOnly(13, 10)))),
            new Period("5", new TimeOnly(13, 15), new TimeOnly(14, 00)),
            new Period("6", new TimeOnly(14, 05), new TimeOnly(14, 50)),
            new Period("7", new TimeOnly(14, 55), new TimeOnly(15, 40)),
        });

        static readonly Schedule tuesdayFriday = new Schedule(new PeriodBase[] {
            new Period("1", new TimeOnly(8, 40), new TimeOnly(9, 25)),
            new Period("2", new TimeOnly(9, 30), new TimeOnly(10, 15)),
            new Period("Registry", new TimeOnly(10, 20), new TimeOnly(10, 50)),
            new Period("3", new TimeOnly(10, 55), new TimeOnly(11, 40)),
            new Lunch(
                new LunchSlot("A Lunch / 4",
                    new Period("A Lunch", new TimeOnly(11, 40), new TimeOnly(12, 20)),
                    new Period("4", new TimeOnly(12, 25), new TimeOnly(13, 10))),
                new LunchSlot("4 / B Lunch",
                    new Period("4", new TimeOnly(11, 45), new TimeOnly(12, 30)),
                    new Period("B Lunch", new TimeOnly(12, 30), new TimeOnly(13, 10)))),
            new Period("5", new TimeOnly(13, 15), new TimeOnly(14, 00)),
            new Period("6", new TimeOnly(14, 05), new TimeOnly(14, 50)),
            new Period("7", new TimeOnly(14, 55), new TimeOnly(15, 40))
        });
        static readonly Dictionary<DayOfWeek, Schedule> regular = new Dictionary<DayOfWeek, Schedule> {
            [DayOfWeek.Monday] = new Schedule(new PeriodBase[] {
                new Period("1", new TimeOnly(8, 40), new TimeOnly(9, 25)),
                new Period("2", new TimeOnly(9, 30), new TimeOnly(10, 15)),
                new Period("Registry", new TimeOnly(10, 20), new TimeOnly(10, 50)),
                new Period("3", new TimeOnly(10, 55), new TimeOnly(11, 40)),
                new Lunch(
                    new LunchSlot("A Lunch / 4",
                        new Period("A Lunch", new TimeOnly(11, 40), new TimeOnly(12, 20)),
                        new Period("4", new TimeOnly(12, 25), new TimeOnly(13, 10))),
                    new LunchSlot("4 / B Lunch",
                        new Period("4", new TimeOnly(11, 45), new TimeOnly(12, 30)),
                        new Period("B Lunch", new TimeOnly(12, 30), new TimeOnly(13, 10)))),
                new Period("5", new TimeOnly(13, 15), new TimeOnly(14, 00)),
                new Period("6", new TimeOnly(14, 05), new TimeOnly(14, 50)),
                new Period("7", new TimeOnly(14, 55), new TimeOnly(15, 40))
            }),
            [DayOfWeek.Tuesday] = tuesdayFriday,
            [DayOfWeek.Wednesday] = new Schedule(new PeriodBase[] {
                new Period("2", new TimeOnly(8, 40), new TimeOnly(10, 10)),
                new Period("Registry", new TimeOnly(10, 15), new TimeOnly(10, 35)),
                new Lunch(
                    new LunchSlot("A Lunch / 4",
                        new Period("A Lunch", new TimeOnly(10, 35), new TimeOnly(11, 15)), 
                        new Period("4", new TimeOnly(11, 20), new TimeOnly(12, 50))),
                    new LunchSlot("4 / B Lunch", 
                        new Period("4", new TimeOnly(10, 40), new TimeOnly(12, 10)),
                        new Period("B Lunch", new TimeOnly(12, 10), new TimeOnly(12, 50)))),
                new Period("6", new TimeOnly(12, 55), new TimeOnly(14, 25)),
            }),
            [DayOfWeek.Thursday] = new Schedule(new PeriodBase[] {
                new Period("1", new TimeOnly(8, 40), new TimeOnly(10, 05)),
                new Period("Registry", new TimeOnly(10, 10), new TimeOnly(10, 30)),
                new Lunch(
                    new LunchSlot("A Lunch / 3",
                        new Period("A Lunch", new TimeOnly(10, 30), new TimeOnly(11, 10)),
                        new Period("3", new TimeOnly(11, 15), new TimeOnly(12, 40))),
                    new LunchSlot("3 / B Lunch",
                        new Period("3", new TimeOnly(10, 35), new TimeOnly(12, 00)),
                        new Period("B Lunch", new TimeOnly(12, 00), new TimeOnly(12, 40)))),
                new Period("5", new TimeOnly(12, 45), new TimeOnly(14, 15)),
                new Period("7", new TimeOnly(14, 15), new TimeOnly(15, 40)),
            }),
            [DayOfWeek.Friday] = tuesdayFriday
        };
        //TODO: Finals
        static readonly DateOnly[] finalsDates = new DateOnly[] { 
        
        };
        static readonly new Dictionary<DayOfWeek, Schedule> finalsSchedule = new Dictionary<DayOfWeek, Schedule> {

        };

        //TODO: SBAC
        static readonly DateOnly[] sbacDates = new DateOnly[] {

        };
        static readonly new Dictionary<DayOfWeek, Schedule> sbacSchedule = new Dictionary<DayOfWeek, Schedule> {

        };

        public static Schedule GetSchedule(DateOnly date) {
            if(NoSchool(date)) return new Schedule(Enumerable.Empty<Period>());
            if (rallies.Contains(date)) {
                return rallySchedule;
            }
            if(firstWeek.Contains(date)) {
                return firstWeekSchedule;
            }
            if(finalsDates.Contains(date)) {
                return finalsSchedule[date.DayOfWeek];
            }
            if (sbacDates.Contains(date)) {
                return sbacSchedule[date.DayOfWeek];
            }
            return regular[date.DayOfWeek];
        }

        static DateOnly schoolStart = new(2022, 8, 17);
        static DateOnly schoolEnd = new(2023, 6, 2);
        static DateOnly[] holidays = new[] {
            new DateOnly(2022, 9, 5),   //Labor day
            new DateOnly(2022, 10, 10), //Columbus day
            new DateOnly(2022, 11, 11), //Veterans day
            new DateOnly(2023, 1, 16),  //MLK
            new DateOnly(2023, 1, 23),  //Lunar NY
            new DateOnly(2023, 2, 20),  //Pres day
            new DateOnly(2023, 5, 29),  //Memorial day
        };
        static DateOnly fallRecessStart = new(2022, 11, 23);
        static DateOnly fallRecessEnd = new(2022, 11, 27);
        static DateOnly winterRecessStart = new(2022, 12, 19);
        static DateOnly winterRecessEnd = new(2023, 1, 2);
        static DateOnly springRecessStart = new(2023, 3, 27);
        static DateOnly springRecessEnd = new(2023, 3, 31);
        public static bool NoSchool(DateOnly date) {
            if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday) {
                return true;
            }

            if (date < schoolStart || date > schoolEnd) return true;
            if (date >= fallRecessStart && date <= fallRecessEnd) return true;
            if (date >= winterRecessStart && date <= winterRecessEnd) return true;
            if (date >= springRecessStart && date <= springRecessEnd) return true;

            if (holidays.Contains(date)) return true;
                
            return false;
        }
    }

    public record Lunch(LunchSlot A, LunchSlot B) : PeriodBase("");
    public record LunchSlot(string Name, Period First, Period Second);
    public record PeriodBase(string Name);
    public record Period(string Name, TimeOnly StartTime, TimeOnly EndTime) : PeriodBase(Name);
    public record Schedule(IEnumerable<PeriodBase> Periods);
}
