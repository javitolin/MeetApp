using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DDay.iCal;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2.Flows;
namespace MeetApp
{
    public partial class _Default : Page
    {
        string clientId = @"1016818064699-m0kivmbto7isf1slt4b12uv6mgefbn0u.apps.googleusercontent.com";
        string clientSecret = @"PM3f5oqE5l8UgOvw6NGqG_W5";
        string userName = "user";//  A string used to identify a user.
        string[] scopes = new string[] { 
            CalendarService.Scope.CalendarReadonly // View your Calendars
        };
        private static CalendarService service;

        DateTime now = DateTime.Now;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /*
         * This function checks if a @startTime is a good time for a meeting 
         * with length @duration while accounting for all the events in @occurs
         */
        private bool isFree(IList<Occurrence> occurs, DateTime startTime, int duration)
        {
            foreach (Occurrence oc in occurs)
            {
                DateTime start = oc.Period.StartTime.Local;
                DateTime end = oc.Period.EndTime.Local;
                if (startTime >= start && startTime <= end)
                    return false;
                else if (startTime < start && startTime.AddMinutes(duration) > start)
                    return false;
            }
            return true;
        }
        protected void loadButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string saveDir = @"\Calendars\";
                string appPath = Request.PhysicalApplicationPath;
                //Getting the duration and max days to check
                int durationMinutes, maxDays;
                bool minutesInputBool = int.TryParse(minutesInput.Text, out durationMinutes);
                bool maxDaysBool = int.TryParse(maxDaysInput.Text, out maxDays);
                if (FileUpload1.HasFile && FileUpload2.HasFile)
                {
                    //Saving both calendars
                    string savePath1 = appPath + saveDir + Server.HtmlEncode(FileUpload1.FileName);
                    FileUpload1.SaveAs(savePath1);
                    string savePath2 = appPath + saveDir + Server.HtmlEncode(FileUpload2.FileName);
                    FileUpload2.SaveAs(savePath2);

                    //Creating the calendars and getting the occurences (events).
                    IICalendarCollection firstCalendar = iCalendar.LoadFromFile(savePath1);
                    IICalendarCollection secondCalendar = iCalendar.LoadFromFile(savePath2);
                    IList<Occurrence> occurrences1 = firstCalendar.GetOccurrences(DateTime.Today, DateTime.Today.AddDays(maxDays));
                    IList<Occurrence> occurrences2 = secondCalendar.GetOccurrences(DateTime.Today, DateTime.Today.AddDays(maxDays));
                    List<DateTime> freeSlots = new List<DateTime>();

                    //This list will contain all the occurences from both calendars
                    List<Occurrence> allOccurences = new List<Occurrence>();
                    //Merging the calendars into @allOccurences
                    mergeCalendars(occurrences1, occurrences2, allOccurences);
                    //Finding free spots
                    findFreeSpots(durationMinutes, maxDays, freeSlots, allOccurences);

                    Label1.Text = "This options are available:<br />";
                    //Printing all the available spots
                    foreach (DateTime dt in freeSlots)
                    {
                        Label1.Text += dt.ToShortDateString() + " at " + dt.ToShortTimeString() + "<br />";
                    }
                }
                minutesInput.Text = "";
                maxDaysInput.Text = "";
            }
        }

        private void findFreeSpots(int durationMinutes, int maxDays, List<DateTime> freeSlots, List<Occurrence> allOccurences)
        {
            DateTime end = now.AddDays(maxDays);
            //Curr day will be today, with rounded hour
            DateTime newNow = now.AddHours(1);
            DateTime currDay = new DateTime(newNow.Year, newNow.Month, newNow.Day, newNow.Hour, 0, 0);
            int numberOfOptions = 0;
            //Iterating through all the options between 8 and 20 minus the minutes for the meeting
            //This way no meeting will be taken place after 20.
            while (numberOfOptions < 5)
            {
                if (currDay.Hour > 8 && currDay.Hour < (20 - durationMinutes / 60))
                {

                    bool isFreeTime = isFree(allOccurences, currDay, durationMinutes);
                    if (isFreeTime)
                    {
                        freeSlots.Add(currDay);
                        numberOfOptions++;
                    }
                }
                currDay = currDay.AddHours(1);
            }
        }

        private static void mergeCalendars(IList<Occurrence> occurrences1, IList<Occurrence> occurrences2, List<Occurrence> allOccurences)
        {
            int firstIterator = 0, secondIterator = 0, lastIndex = 0;
            while (firstIterator < occurrences1.Count || secondIterator < occurrences2.Count)
            {
                if (firstIterator >= occurrences1.Count)
                {
                    if (allOccurences[lastIndex].CompareTo(occurrences2[secondIterator]) != 0)
                    {

                        allOccurences.Add(occurrences2[secondIterator]);
                        lastIndex++;
                    }
                    secondIterator++;
                    continue;
                }
                if (secondIterator >= occurrences2.Count)
                {
                    if (allOccurences[lastIndex].CompareTo(occurrences1[firstIterator]) != 0)
                    {
                        allOccurences.Add(occurrences1[firstIterator]);
                        lastIndex++;
                    }
                    firstIterator++;
                    continue;
                }
                Occurrence firstOcc;
                if (occurrences1[firstIterator].Period.StartTime.Date < occurrences2[secondIterator].Period.StartTime.Date)
                    firstOcc = occurrences1[firstIterator++];
                else
                    firstOcc = occurrences2[secondIterator++];

                if (allOccurences.Count > 0)
                {
                    if (allOccurences[lastIndex].CompareTo(firstOcc) != 0)
                    {
                        allOccurences.Add(firstOcc);
                        lastIndex++;
                    }
                }
                else
                {
                    allOccurences.Add(firstOcc);
                }
            }
        }

        protected void googleLogin_Click(object sender, EventArgs e)
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
                scopes,
                userName,
                CancellationToken.None,
                //new AuthStorageGoogle(@"C:\Users\Javier\documents\visual studio 2012\Projects\MeetApp\MeetApp\App_Data\MeetAppDB.mdf", "javitolin", "manajama12", "MeetAppDB", "userCredentials")).Result;
                new AuthDataStorageFile(@"C:\Users\Javier\Documents\googleCredentials")).Result;

            googleGetCalendars(credential);

        }

        private void googleGetCalendars(UserCredential credential)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer();
            initializer.HttpClientInitializer = credential;
            initializer.ApplicationName = "meetapp-997";
            service = new CalendarService(initializer);
            allCalendarItemsText.Text = "";
            IList<CalendarListEntry> list = service.CalendarList.List().Execute().Items;
            foreach (Google.Apis.Calendar.v3.Data.CalendarListEntry calendar in list)
            {
                googleGetEventsFromCalendar(calendar);
            }
        }

        private void googleGetEventsFromCalendar(Google.Apis.Calendar.v3.Data.CalendarListEntry calendar)
        {
            EventsResource.ListRequest request = service.Events.List(calendar.Id);
            request.TimeMin = now;
            foreach (Google.Apis.Calendar.v3.Data.Event calendarEvent in request.Execute().Items)
            {
                DateTime start = (calendarEvent.Start.DateTime.HasValue ? calendarEvent.Start.DateTime.Value : now);
                DateTime end = (calendarEvent.End.DateTime.HasValue ? calendarEvent.End.DateTime.Value : now);
                allCalendarItemsText.Text += "Summary: " + calendarEvent.Summary + " from " + start.ToLocalTime() + " to " + end.ToLocalTime() + "<br />";
            }
        }

        protected void outlookButton_Click(object sender, EventArgs e)
        {
            Microsoft.Office.Interop.Outlook.Application oApp = null;
            Microsoft.Office.Interop.Outlook.NameSpace mapiNamespace = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder CalendarFolder = null;
            Microsoft.Office.Interop.Outlook.Items outlookCalendarItems = null;

            oApp = new Microsoft.Office.Interop.Outlook.Application();
            mapiNamespace = oApp.GetNamespace("MAPI"); ;
            CalendarFolder = mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);
            outlookCalendarItems = CalendarFolder.Items;
            outlookCalendarItems.IncludeRecurrences = true;
            allCalendarItemsText.Text = "";
            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                allCalendarItemsText.Text += item.Subject + " -> " + item.Start.ToLongDateString();
            }
        }
    }
}