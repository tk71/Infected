using System;
using InfectedLibrary.Enums;
using InfectedLibrary.Models;

namespace InfectedLibrary.Data
{
    internal static class EmployeeGenerator
    {
        private static string[] FirstNamesFemale { get; set; }
        private static string[] FirstNamesMale { get; set; }
        private static string[] LastNames { get; set; }

        static EmployeeGenerator()
        {
            // female first names are top 300 from the US Social Security Frequently Occuring Given Names from 2000
            FirstNamesFemale = new string[] { "Emily","Hannah","Madison","Ashley","Sarah","Alexis","Samantha","Jessica","Elizabeth","Taylor",
                "Lauren","Alyssa","Kayla","Abigail","Brianna","Olivia","Emma","Megan","Grace","Victoria","Rachel","Anna","Sydney","Destiny",
                "Morgan","Jennifer","Jasmine","Haley","Julia","Kaitlyn","Nicole","Amanda","Katherine","Natalie","Hailey","Alexandra","Savannah",
                "Chloe","Rebecca","Stephanie","Maria","Sophia","Mackenzie","Allison","Isabella","Mary","Amber","Danielle","Gabrielle","Jordan",
                "Brooke","Michelle","Sierra","Katelyn","Andrea","Madeline","Sara","Kimberly","Courtney","Erin","Brittany","Vanessa","Jenna",
                "Jacqueline","Caroline","Faith","Makayla","Bailey","Paige","Shelby","Melissa","Kaylee","Christina","Trinity","Mariah","Caitlin",
                "Autumn","Marissa","Angela","Breanna","Catherine","Zoe","Briana","Jada","Laura","Claire","Alexa","Kelsey","Kathryn","Leslie",
                "Alexandria","Sabrina","Mia","Isabel","Molly","Katie","Leah","Gabriella","Cheyenne","Cassandra","Tiffany","Erica","Lindsey",
                "Kylie","Amy","Diana","Cassidy","Mikayla","Ariana","Margaret","Kelly","Miranda","Maya","Melanie","Audrey","Jade","Gabriela",
                "Caitlyn","Angel","Jillian","Alicia","Jocelyn","Erika","Lily","Heather","Madelyn","Adriana","Arianna","Lillian","Kiara","Riley",
                "Crystal","Mckenzie","Meghan","Skylar","Ana","Britney","Angelica","Kennedy","Chelsea","Daisy","Kristen","Veronica","Isabelle",
                "Summer","Hope","Brittney","Lydia","Hayley","Evelyn","Bethany","Shannon","Karen","Michaela","Jamie","Daniela","Angelina","Kaitlin",
                "Karina","Sophie","Sofia","Diamond","Payton","Cynthia","Alexia","Valerie","Monica","Peyton","Carly","Bianca","Hanna","Brenda",
                "Rebekah","Alejandra","Mya","Avery","Brooklyn","Ashlyn","Lindsay","Ava","Desiree","Alondra","Camryn","Ariel","Naomi","Jordyn",
                "Kendra","Mckenna","Holly","Julie","Kendall","Kara","Jasmin","Selena","Esmeralda","Amaya","Kylee","Maggie","Makenzie","Claudia",
                "Kyra","Cameron","Karla","Kathleen","Abby","Delaney","Amelia","Casey","Serena","Savanna","Aaliyah","Giselle","Mallory","April",
                "Adrianna","Raven","Christine","Kristina","Nina","Asia","Natalia","Valeria","Aubrey","Lauryn","Kate","Patricia","Jazmin","Rachael",
                "Katelynn","Cierra","Alison","Nancy","Macy","Elena","Kyla","Katrina","Jazmine","Joanna","Tara","Gianna","Juliana","Fatima",
                "Sadie","Allyson","Gracie","Guadalupe","Genesis","Yesenia","Julianna","Skyler","Tatiana","Alexus","Alana","Elise","Kirsten","Nadia",
                "Sandra","Ruby","Dominique","Haylee","Jayla","Tori","Cindy","Ella","Sidney","Tessa","Carolina","Jaqueline","Camille","Carmen",
                "Whitney","Vivian","Priscilla","Bridget","Celeste","Kiana","Makenna","Alissa","Madeleine","Miriam","Natasha","Ciara","Cecilia",
                "Kassandra","Mercedes","Reagan","Aliyah","Josephine","Charlotte","Rylee","Shania","Kira","Meredith","Eva","Lisa","Dakota","Hallie",
                "Anne","Rose","Liliana" };

            // male first names are top 300 from the US Social Security Frequently Occuring Given Names from 2000
            FirstNamesMale = new string[] { "Jacob","Michael","Matthew","Joshua","Christopher","Nicholas","Andrew","Joseph","Daniel","Tyler","William",
                "Brandon","Ryan","John","Zachary","David","Anthony","James","Justin","Alexander","Jonathan","Christian","Austin","Dylan","Ethan",
                "Benjamin","Noah","Samuel","Robert","Nathan","Cameron","Kevin","Thomas","Jose","Hunter","Jordan","Kyle","Caleb","Jason","Logan",
                "Aaron","Eric","Brian","Gabriel","Adam","Jack","Isaiah","Juan","Luis","Connor","Charles","Elijah","Isaac","Steven","Evan","Jared",
                "Sean","Timothy","Luke","Cody","Nathaniel","Alex","Seth","Mason","Richard","Carlos","Angel","Patrick","Devin","Bryan","Cole","Jackson",
                "Ian","Garrett","Trevor","Jesus","Chase","Adrian","Mark","Blake","Sebastian","Antonio","Lucas","Jeremy","Gavin","Miguel","Julian",
                "Dakota","Alejandro","Jesse","Dalton","Bryce","Tanner","Kenneth","Stephen","Jake","Victor","Spencer","Marcus","Paul","Brendan","Jeremiah",
                "Xavier","Jeffrey","Tristan","Jalen","Jorge","Edward","Riley","Colton","Wyatt","Joel","Maxwell","Aidan","Travis","Shane","Colin","Dominic",
                "Carson","Vincent","Derek","Oscar","Grant","Eduardo","Peter","Henry","Parker","Collin","Hayden","George","Bradley","Mitchell","Devon",
                "Ricardo","Shawn","Taylor","Nicolas","Gregory","Francisco","Liam","Kaleb","Preston","Erik","Alexis","Owen","Omar","Diego","Dustin",
                "Corey","Fernando","Clayton","Carter","Ivan","Jaden","Javier","Alec","Johnathan","Scott","Manuel","Cristian","Alan","Raymond","Brett",
                "Max","Andres","Gage","Mario","Dawson","Dillon","Cesar","Wesley","Levi","Jakob","Chandler","Martin","Malik","Edgar","Sergio","Trenton",
                "Josiah","Nolan","Marco","Peyton","Harrison","Hector","Micah","Roberto","Drew","Brady","Erick","Conner","Jonah","Casey","Jayden","Edwin",
                "Emmanuel","Andre","Phillip","Brayden","Landon","Giovanni","Bailey","Ronald","Braden","Damian","Donovan","Ruben","Frank","Gerardo","Pedro",
                "Andy","Chance","Abraham","Calvin","Trey","Cade","Donald","Derrick","Payton","Darius","Enrique","Keith","Raul","Jaylen","Troy","Jonathon",
                "Cory","Marc","Eli","Skyler","Rafael","Trent","Griffin","Colby","Johnny","Chad","Armando","Kobe","Caden","Marcos","Cooper","Elias","Brenden",
                "Israel","Avery","Zane","Dante","Josue","Zackary","Allen","Mathew","Dennis","Leonardo","Ashton","Philip","Julio","Miles","Damien","Ty","Gustavo",
                "Drake","Jaime","Simon","Jerry","Curtis","Kameron","Lance","Brock","Bryson","Alberto","Dominick","Jimmy","Kaden","Douglas","Gary","Brennan",
                "Zachery","Randy","Louis","Larry","Nickolas","Albert","Tony","Fabian","Keegan","Saul","Danny","Tucker","Myles","Damon","Arturo","Corbin",
                "Deandre","Ricky","Kristopher","Lane","Pablo","Darren","Jarrett","Zion" };

            // last names are top 500 from the US Census Bureau Frequently Occuring Surnames from 2000
            LastNames = new string[] { "SMITH","JOHNSON","WILLIAMS","BROWN","JONES","MILLER","DAVIS","GARCIA","RODRIGUEZ","WILSON","MARTINEZ",
                "ANDERSON","TAYLOR","THOMAS","HERNANDEZ","MOORE","MARTIN","JACKSON","THOMPSON","WHITE","LOPEZ","LEE","GONZALEZ","HARRIS",
                "CLARK","LEWIS","ROBINSON","WALKER","PEREZ","HALL","YOUNG","ALLEN","SANCHEZ","WRIGHT","KING","SCOTT","GREEN","BAKER","ADAMS",
                "NELSON","HILL","RAMIREZ","CAMPBELL","MITCHELL","ROBERTS","CARTER","PHILLIPS","EVANS","TURNER","TORRES","PARKER","COLLINS",
                "EDWARDS","STEWART","FLORES","MORRIS","NGUYEN","MURPHY","RIVERA","COOK","ROGERS","MORGAN","PETERSON","COOPER","REED","BAILEY",
                "BELL","GOMEZ","KELLY","HOWARD","WARD","COX","DIAZ","RICHARDSON","WOOD","WATSON","BROOKS","BENNETT","GRAY","JAMES","REYES",
                "CRUZ","HUGHES","PRICE","MYERS","LONG","FOSTER","SANDERS","ROSS","MORALES","POWELL","SULLIVAN","RUSSELL","ORTIZ","JENKINS",
                "GUTIERREZ","PERRY","BUTLER","BARNES","FISHER","HENDERSON","COLEMAN","SIMMONS","PATTERSON","JORDAN","REYNOLDS","HAMILTON",
                "GRAHAM","KIM","GONZALES","ALEXANDER","RAMOS","WALLACE","GRIFFIN","WEST","COLE","HAYES","CHAVEZ","GIBSON","BRYANT","ELLIS",
                "STEVENS","MURRAY","FORD","MARSHALL","OWENS","MCDONALD","HARRISON","RUIZ","KENNEDY","WELLS","ALVAREZ","WOODS","MENDOZA",
                "CASTILLO","OLSON","WEBB","WASHINGTON","TUCKER","FREEMAN","BURNS","HENRY","VASQUEZ","SNYDER","SIMPSON","CRAWFORD","JIMENEZ",
                "PORTER","MASON","SHAW","GORDON","WAGNER","HUNTER","ROMERO","HICKS","DIXON","HUNT","PALMER","ROBERTSON","BLACK","HOLMES",
                "STONE","MEYER","BOYD","MILLS","WARREN","FOX","ROSE","RICE","MORENO","SCHMIDT","PATEL","FERGUSON","NICHOLS","HERRERA","MEDINA",
                "RYAN","FERNANDEZ","WEAVER","DANIELS","STEPHENS","GARDNER","PAYNE","KELLEY","DUNN","PIERCE","ARNOLD","TRAN","SPENCER","PETERS",
                "HAWKINS","GRANT","HANSEN","CASTRO","HOFFMAN","HART","ELLIOTT","CUNNINGHAM","KNIGHT","BRADLEY","CARROLL","HUDSON","DUNCAN",
                "ARMSTRONG","BERRY","ANDREWS","JOHNSTON","RAY","LANE","RILEY","CARPENTER","PERKINS","AGUILAR","SILVA","RICHARDS","WILLIS",
                "MATTHEWS","CHAPMAN","LAWRENCE","GARZA","VARGAS","WATKINS","WHEELER","LARSON","CARLSON","HARPER","GEORGE","GREENE","BURKE",
                "GUZMAN","MORRISON","MUNOZ","JACOBS","OBRIEN","LAWSON","FRANKLIN","LYNCH","BISHOP","CARR","SALAZAR","AUSTIN","MENDEZ","GILBERT",
                "JENSEN","WILLIAMSON","MONTGOMERY","HARVEY","OLIVER","HOWELL","DEAN","HANSON","WEBER","GARRETT","SIMS","BURTON","FULLER","SOTO",
                "MCCOY","WELCH","CHEN","SCHULTZ","WALTERS","REID","FIELDS","WALSH","LITTLE","FOWLER","BOWMAN","DAVIDSON","MAY","DAY","SCHNEIDER",
                "NEWMAN","BREWER","LUCAS","HOLLAND","WONG","BANKS","SANTOS","CURTIS","PEARSON","DELGADO","VALDEZ","PENA","RIOS","DOUGLAS","SANDOVAL",
                "BARRETT","HOPKINS","KELLER","GUERRERO","STANLEY","BATES","ALVARADO","BECK","ORTEGA","WADE","ESTRADA","CONTRERAS","BARNETT","CALDWELL",
                "SANTIAGO","LAMBERT","POWERS","CHAMBERS","NUNEZ","CRAIG","LEONARD","LOWE","RHODES","BYRD","GREGORY","SHELTON","FRAZIER","BECKER",
                "MALDONADO","FLEMING","VEGA","SUTTON","COHEN","JENNINGS","PARKS","MCDANIEL","WATTS","BARKER","NORRIS","VAUGHN","VAZQUEZ","HOLT",
                "SCHWARTZ","STEELE","BENSON","NEAL","DOMINGUEZ","HORTON","TERRY","WOLFE","HALE","LYONS","GRAVES","HAYNES","MILES","PARK","WARNER",
                "PADILLA","BUSH","THORNTON","MCCARTHY","MANN","ZIMMERMAN","ERICKSON","FLETCHER","MCKINNEY","PAGE","DAWSON","JOSEPH","MARQUEZ","REEVES",
                "KLEIN","ESPINOZA","BALDWIN","MORAN","LOVE","ROBBINS","HIGGINS","BALL","CORTEZ","LE","GRIFFITH","BOWEN","SHARP","CUMMINGS","RAMSEY",
                "HARDY","SWANSON","BARBER","ACOSTA","LUNA","CHANDLER","BLAIR","DANIEL","CROSS","SIMON","DENNIS","OCONNOR","QUINN","GROSS","NAVARRO",
                "MOSS","FITZGERALD","DOYLE","MCLAUGHLIN","ROJAS","RODGERS","STEVENSON","SINGH","YANG","FIGUEROA","HARMON","NEWTON","PAUL","MANNING",
                "GARNER","MCGEE","REESE","FRANCIS","BURGESS","ADKINS","GOODMAN","CURRY","BRADY","CHRISTENSEN","POTTER","WALTON","GOODWIN","MULLINS",
                "MOLINA","WEBSTER","FISCHER","CAMPOS","AVILA","SHERMAN","TODD","CHANG","BLAKE","MALONE","WOLF","HODGES","JUAREZ","GILL","FARMER","HINES",
                "GALLAGHER","DURAN","HUBBARD","CANNON","MIRANDA","WANG","SAUNDERS","TATE","MACK","HAMMOND","CARRILLO","TOWNSEND","WISE","INGRAM","BARTON",
                "MEJIA","AYALA","SCHROEDER","HAMPTON","ROWE","PARSONS","FRANK","WATERS","STRICKLAND","OSBORNE","MAXWELL","CHAN","DELEON","NORMAN","HARRINGTON",
                "CASEY","PATTON","LOGAN","BOWERS","MUELLER","GLOVER","FLOYD","HARTMAN","BUCHANAN","COBB","FRENCH","KRAMER","MCCORMICK","CLARKE","TYLER",
                "GIBBS","MOODY","CONNER","SPARKS","MCGUIRE","LEON","BAUER","NORTON","POPE","FLYNN","HOGAN","ROBLES","SALINAS","YATES","LINDSEY","LLOYD",
                "MARSH","MCBRIDE","OWEN","SOLIS","PHAM","LANG","PRATT"};
        }

        public static Employee NewEmployee()
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var sex = rnd.Next(0, 2) == 0 ? "F" : "M";
            var fname = sex == "F" ? FirstNamesFemale[rnd.Next(0, FirstNamesFemale.Length)] : FirstNamesMale[rnd.Next(0, FirstNamesMale.Length)];
            var lname = LastNames[rnd.Next(0, LastNames.Length)];
            lname = lname.Replace(lname.Substring(1), lname.Substring(1).ToLower());

            return new Employee()
            {
                Id = Guid.NewGuid().ToString().ToUpper().Replace("-", string.Empty).Substring(0, 11),
                FirstName = fname,
                LastName = lname,
                Sex = sex,
                Status = InfectionState.Well
            };
        }
    }
}
