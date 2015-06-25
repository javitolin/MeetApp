using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DDay.iCal;
namespace MeetApp
{
    public partial class _Default : Page
    {
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
    }
}