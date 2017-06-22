using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using church.ccv.Hr.Data;
using church.ccv.Hr.Model;
using Rock;
using Rock.Attribute;
using Rock.Communication;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.church_ccv.Hr
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Time Card Quick Entry" )]
    [Category( "CCV > Time Card" )]
    [Description( "Block for displaying quick entry buttons for the current pay period timecard." )]

    [DateField("First Pay Period Start Date", "The Start Date of the first Payroll Period.  This will determine the daterange of subsequent Pay Periods.", false, "2014-12-15")]

    public partial class TimeCardQuickEntry : Rock.Web.UI.RockBlock
    {
        #region Base Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                // Get the current period time card
                TimeCard currentTimeCard = GetCurrentTimeCard();

                // If no current time card, show Create Time Card button
                if ( currentTimeCard == null )
                {
                    pnlQuickEntry.Visible = false;
                    pnlCreateTimeCard.Visible = true;
                }
                else if ( currentTimeCard != null )
                {
                    // Set the current timecard period Label
                    lblCurrentTimePeriod.Text = String.Format( "Pay Period: {0}", currentTimeCard.TimeCardPayPeriod );

                    // Set clocked in status text
                    lblClockedStatus.Text = currentTimeCard.GetClockedInStatusText();

                    // Show / Hide relavent buttons for Clocking In / out
                    switch ( currentTimeCard.ClockedInStatus )
                    {
                        case ClockedInStatus.In:
                            lbTimeIn.Visible = false;
                            lbLunchIn.Visible = false;
                            lbLunchOut.Visible = true;
                            lbTimeOut.Visible = true;
                            break;
                        case ClockedInStatus.LunchOut:
                            lbLunchOut.Visible = false;
                            lbTimeOut.Visible = false;
                            lbTimeIn.Visible = false;
                            lbLunchIn.Visible = true;
                            break;
                        case ClockedInStatus.LunchIn:
                            lbLunchIn.Visible = false;
                            lbLunchOut.Visible = false;
                            lbTimeIn.Visible = false;
                            lbTimeOut.Visible = true;
                            break;
                        case ClockedInStatus.Out:
                            lbTimeIn.Visible = true;
                            lbLunchIn.Visible = false;
                            lbLunchOut.Visible = false;
                            lbTimeOut.Visible = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        #region repeater helpers

        /// <summary>
        /// Formats the time card time.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="zeroAsDash">if set to <c>true</c> [zero as dash].</param>
        /// <returns></returns>
        public string FormatTimeCardTime( DateTime? dateTime, bool zeroAsDash = false )
        {
            if ( dateTime.HasValue )
            {
                if ( dateTime.Value.TimeOfDay == TimeSpan.Zero && zeroAsDash )
                {
                    return "-";
                }
                else
                {
                    return dateTime.Value.ToString( "hh:mmtt" );
                }
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Formats the time card hours.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <returns></returns>
        public string FormatTimeCardHours( decimal? hours )
        {
            if ( hours.HasValue && hours != 0 )
            {
                TimeSpan timeSpan = TimeSpan.FromHours( Convert.ToDouble( hours ) );
                return timeSpan.TotalHours.ToString( "0.##" );
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the click event of the lbClockIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbTimeIn_Click(object sender, EventArgs e)
        {
            DateTime timeIn = DateTime.Now;
            //UpdateCurrentTimeCard(clockInTime, lunchOutTime, lunchInTime, clockOutTime);
            

            // Add the current DateTime to today's time card in the Time In field
            
            lblClockedStatus.Text = "Clocked In at (time)";
            lbTimeIn.Visible = false;
            lbLunchOut.Visible = true;
            lbTimeOut.Visible = true;
        }

        /// <summary>
        /// Handles the click event of the lbLunchOut control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbLunchOut_Click(object sender, EventArgs e)
        {

            // Add the current DateTime to todays time card in the Lunch Out field

            lblClockedStatus.Text = "Lunch Out at (time)";
            lbLunchOut.Visible = false;
            lbTimeOut.Visible = false;
            lbLunchIn.Visible = true;
        }

        /// <summary>
        /// Handles the click event of the lbLunchIn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbLunchIn_Click(object sender, EventArgs e)
        {

            // Add the current DateTime to todays time card in the Lunch In field

            lblClockedStatus.Text = "Lunch In at (time)";
            lbLunchIn.Visible = false;
            lbTimeOut.Visible = true;
        }

        /// <summary>
        /// Handles the click event of the lbClockOut Control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbTimeOut_Click(object sender, EventArgs e)
        {

            // Add the current DateTime to todays time card in the Clock Out field

            lbLunchOut.Visible = false;
            lbTimeOut.Visible = false;
            lblClockedStatus.Text = "Time entry completed for today";

        }

        /// <summary>
        /// Handles the click event of the lbCreateTimeCard Control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbCreateTimeCard_Click(object sender, EventArgs e )
        {


            // create time card
            //currentEmployeeTimeCard = new TimeCard();
            //currentEmployeeTimeCard.TimeCardPayPeriodId = currentPayPeriod.Id;
            //currentEmployeeTimeCard.TimeCardStatus = TimeCardStatus.InProgress;
            //currentEmployeeTimeCard.PersonAliasId = this.CurrentPersonAliasId.Value;
            //currentEmployeeTimeCard.TimeCardDays = new List<TimeCardDay>();
            //timeCardService.Add( currentEmployeeTimeCard );



        }



        #endregion

        #region Methods

        /// <summary>
        /// Get current pay period
        /// </summary>
        private TimeCard GetCurrentTimeCard()
        {
            var hrContext = new HrContext();
            var timeCardService = new TimeCardService( hrContext );
            var timeCardPayPeriodService = new TimeCardPayPeriodService( hrContext );
            DateTime? firstPayPeriodStartDate = this.GetAttributeValue( "FirstPayPeriodStartDate" ).AsDateTime();
            if ( !firstPayPeriodStartDate.HasValue )
            {
                lblInformationMessage.Text += "The first pay period start date must be set in block settings<br />";
                ShowInformationMessage();
                return null;
            }

            var qry = timeCardService.Queryable().Where( a => a.PersonAlias.PersonId == this.CurrentPersonId );

            // Check if employee has timecard for current pay period and return it, return null if no timecard found
            var currentPayPeriod = timeCardPayPeriodService.EnsureCurrentPayPeriod( firstPayPeriodStartDate.Value );
            var currentEmployeeTimeCard = qry.Where( a => a.TimeCardPayPeriodId == currentPayPeriod.Id ).FirstOrDefault();
            if ( currentEmployeeTimeCard == null )
            {
                return null;
            }
            return currentEmployeeTimeCard;
        }

        private void UpdateCurrentTimeCard(string currentTimeCardId, DateTime timeIn, DateTime lunchOut, DateTime lunchIn, DateTime timeOut)
        {
            var hrContext = new HrContext();
            var timeCardDayService = new TimeCardDayService( hrContext );
            var timeCardDay = timeCardDayService.Get( currentTimeCardId.AsInteger() );

            timeCardDay.StartDateTime = timeIn;
            timeCardDay.LunchStartDateTime = lunchOut;
            timeCardDay.LunchEndDateTime = lunchIn;
            timeCardDay.EndDateTime = timeOut;


        }

        /// <summary>
        /// Show Information Message panel and hide other panels
        /// </summary>
        private void ShowInformationMessage()
        {
            pnlInformationMessage.Visible = true;
            pnlCreateTimeCard.Visible = false;
            pnlQuickEntry.Visible = false;
        }

     
        #endregion




    }
}