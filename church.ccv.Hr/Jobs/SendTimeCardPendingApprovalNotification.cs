using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using church.ccv.Hr.Data;
using church.ccv.Hr.Model;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Jobs;
using Rock.Model;

namespace church.ccv.Hr.Jobs
{
    /// <summary>
    /// Sends out reminders to TimeCard Approvers that they have pending approvals waiting
    /// </summary>
    [SystemEmailField( "Notification Email Template", required: true, order: 0 )]
    [DisallowConcurrentExecution]

    public class SendTimeCardPendingApprovalNotification : IJob
    {
        List<NotificationItem> _notificationList = new List<NotificationItem>();
        List<TimeCard> _submittedTimeCardList = new List<TimeCard>();

        /// <summary>
        /// Empty constructor for job initialization
        /// <para>
        /// Jobs require a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        ///  </summary>
        public SendTimeCardPendingApprovalNotification()
        {
        }

        /// <summary>
        /// Executes the specified context
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute( IJobExecutionContext context )
        {
            HrContext hrContext = new HrContext();


            JobDataMap dataMap = context.JobDetail.JobDataMap;
            Guid? systemEmailGuid = dataMap.GetString( "NotificationEmailTemplate" ).AsGuidOrNull();



            if ( systemEmailGuid.HasValue )
            {

                TimeCardPayPeriodService timeCardPayPeriodService = new TimeCardPayPeriodService( hrContext );
                TimeCardService timeCardService = new TimeCardService( hrContext );
                PersonService personService = new PersonService( hrContext );

                // Get Current Pay period
                var currentPayPeriodQry = timeCardPayPeriodService.Queryable().OrderByDescending( a => a.StartDate ).FirstOrDefault();

                // get timecards
                var timeCardsQry = timeCardService.Queryable().Where( a => a.TimeCardPayPeriodId == currentPayPeriodQry.Id );

                foreach ( var timeCard in timeCardsQry )
                {
                    // If TimeCardStatus "Submitted" add to _submittedTimeCardList 
                    if ( timeCard.TimeCardStatus == TimeCardStatus.Submitted )
                    {
                        _submittedTimeCardList.Add( timeCard );

                        // add approver to _notificationList
                        NotificationItem notification = new NotificationItem();
                        notification.Person = timeCard.SubmittedToPersonAlias.Person;
                        _notificationList.Add( notification );
                    }
                }

                // Todo
                // Send email to Approver(s) for timecards in "Submitted" status



            }






        }

    }
}
