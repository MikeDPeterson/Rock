﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using Rock;
using Rock.Attribute;
using Rock.Constants;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

/* NOTE: CCV Specific Changes in this file
- An option to prevent people that have an "Alert Note" from adding themselves to a group.  For example, a notorious CCV Member with a bad history, kept signing himself up for groups, and was doing it to just mess with people and cause problems (or something like that)
https://github.com/ccvonline/CCVRockit/commit/aa61d2817755844fe126f3a7c282e5abecaa8284 and https://github.com/ccvonline/CCVRockit/commit/31bb91992f80b9c631bac4f853c7c04ce549e5c8 (a bug fix for it)

-- A bunch of general bug fixes.  I don't think any of these are related to the AlertNote feature
https://github.com/ccvonline/CCVRockit/commit/b27d48d6e918f92a7e7960febdfab1547732e753

-- The above changes could be turned into Pull Requests. If core takes these pull requests, we can start using the core version of this block.  Otherwise, we'll just have to be aware of the CCV changes
when updating to future release of core
     
*/

namespace RockWeb.Blocks.Groups
{
    /// <summary>
    /// 
    /// </summary>
    [DisplayName( "Group Registration" )]
    [Category( "Groups" )]
    [Description( "Allows a person to register for a group." )]

    [CustomRadioListField("Mode", "The mode to use when displaying registration details.", "Simple^Simple,Full^Full,FullSpouse^Full With Spouse", true, "Simple", "", 0)]
    [CustomRadioListField( "Group Member Status", "The group member status to use when adding person to group (default: 'Pending'.)", "2^Pending,1^Active,0^Inactive", true, "2", "", 1 )]
    [DefinedValueField( "2E6540EA-63F0-40FE-BE50-F2A84735E600", "Connection Status", "The connection status to use for new individuals (default: 'Web Prospect'.)", true, false, "368DD475-242C-49C4-A42C-7278BE690CC2", "", 2 )]
    [DefinedValueField( "8522BADD-2871-45A5-81DD-C76DA07E2E7E", "Record Status", "The record status to use for new individuals (default: 'Pending'.)", true, false, "283999EC-7346-42E3-B807-BCE9B2BABB49", "", 3 )]
    [WorkflowTypeField( "Workflow", "An optional workflow to start when registration is created. The GroupMember will set as the workflow 'Entity' when processing is started.", false, false, "", "", 4 )]
    [BooleanField( "Enable Debug", "Shows the fields available to merge in lava.", false, "", 5 )]
    [CodeEditorField( "Lava Template", "The lava template to use to format the group details.", CodeEditorMode.Lava, CodeEditorTheme.Rock, 400, true, @"
", "", 6  )]
    [LinkedPage("Result Page", "An optional page to redirect user to after they have been registered for the group.", false, "", "", 7)]
    [CodeEditorField( "Result Lava Template", "The lava template to use to format result message after user has been registered. Will only display if user is not redirected to a Result Page ( previous setting ).", CodeEditorMode.Lava, CodeEditorTheme.Rock, 400, true, @"
", "", 8 )]
    [CustomRadioListField( "Auto Fill Form", "If set to FALSE then the form will not load the context of the logged in user (default: 'True'.)", "true^True,false^False", true, "true", "", 9 )]
    [TextField( "Register Button Alt Text", "Alternate text to use for the Register button (default is 'Register').", false, "", "", 10 )]
    [WorkflowTypeField( "Alert Note Re-Route Workflow", "If the person has an alert note, a workflow to run instead of registering them to the group. A GroupMember with the person and group will be set as the workflow 'Entity' when processing is started.", false, false, "", "", 11 )]
    public partial class GroupRegistration : RockBlock
    {
        #region Fields

        RockContext _rockContext = null;
        string _mode = "Simple";
        Group _group = null;
        GroupTypeRole _defaultGroupRole = null;
        DefinedValueCache _dvcConnectionStatus = null;
        DefinedValueCache _dvcRecordStatus = null;
        DefinedValueCache _married = null;
        DefinedValueCache _homeAddressType = null;
        DefinedValueCache _previousAddressType = null;
        GroupTypeCache _familyType = null;
        GroupTypeRoleCache _adultRole = null;
        bool _autoFill = true;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is simple.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is simple; otherwise, <c>false</c>.
        /// </value>
        protected bool IsSimple
        {
            get
            {
                return _mode == "Simple";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is full; otherwise, <c>false</c>.
        /// </value>
        protected bool IsFull
        {
            get
            {
                return _mode == "Full";
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full with spouse.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is full with spouse; otherwise, <c>false</c>.
        /// </value>
        protected bool IsFullWithSpouse
        {
            get
            {
                return _mode == "FullSpouse";
            }
        }

        #endregion

        #region Control Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // this event gets fired after block settings are updated. it's nice to repaint the screen if these settings would alter it
            this.BlockUpdated += Block_BlockUpdated;
            this.AddConfigurationUpdateTrigger( upnlContent );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !CheckSettings() )
            {
                nbNotice.Visible = true;
                pnlView.Visible = false;
            }
            else
            {
                nbNotice.Visible = false;
                pnlView.Visible = true;

                if ( !Page.IsPostBack )
                {
                    ShowDetails();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the BlockUpdated event of the Block control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Block_BlockUpdated( object sender, EventArgs e )
        {
            ShowDetails();
        }

        /// <summary>
        /// Handles the Click event of the btnRegister control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnRegister_Click( object sender, EventArgs e )
        {
            if ( Page.IsValid )
            {
                var rockContext = new RockContext();
                var personService = new PersonService( rockContext );

                Person person = null;
                Person spouse = null;
                Group family = null;
                GroupLocation homeLocation = null;

                var changes = new List<string>();
                var spouseChanges = new List<string>();
                var familyChanges = new List<string>();

                // Only use current person if the name entered matches the current person's name and autofill mode is true
                if ( _autoFill )
                {
                    if ( CurrentPerson != null &&
                        tbFirstName.Text.Trim().Equals( CurrentPerson.FirstName.Trim(), StringComparison.OrdinalIgnoreCase ) &&
                        tbLastName.Text.Trim().Equals( CurrentPerson.LastName.Trim(), StringComparison.OrdinalIgnoreCase ) )
                    {
                        person = personService.Get( CurrentPerson.Id );
                    }
                }

                // Try to find person by name/email 
                if ( person == null )
                {
                    var matches = personService.GetByMatch( tbFirstName.Text.Trim(), tbLastName.Text.Trim(), tbEmail.Text.Trim() );
                    if ( matches.Count() == 1 )
                    {
                        person = matches.First();
                    }
                }

                // Check to see if this is a new person
                if ( person == null )
                {
                    // If so, create the person and family record for the new person
                    person = new Person();
                    person.FirstName = tbFirstName.Text.Trim();
                    person.LastName = tbLastName.Text.Trim();
                    person.Email = tbEmail.Text.Trim();
                    person.IsEmailActive = true;
                    person.EmailPreference = EmailPreference.EmailAllowed;
                    person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
                    person.ConnectionStatusValueId = _dvcConnectionStatus.Id;
                    person.RecordStatusValueId = _dvcRecordStatus.Id;
                    person.Gender = Gender.Unknown;

                    family = PersonService.SaveNewPerson( person, rockContext, _group.CampusId, false );
                }
                else
                {
                    // updating current existing person
                    History.EvaluateChange( changes, "Email", person.Email, tbEmail.Text );
                    person.Email = tbEmail.Text;

                    // Get the current person's families
                    var families = person.GetFamilies( rockContext );

                    // If address can being entered, look for first family with a home location
                    if ( !IsSimple )
                    {
                        foreach ( var aFamily in families )
                        {
                            homeLocation = aFamily.GroupLocations
                                .Where( l =>
                                    l.GroupLocationTypeValueId == _homeAddressType.Id &&
                                    l.IsMappedLocation )
                                .FirstOrDefault();
                            if ( homeLocation != null )
                            {
                                family = aFamily;
                                break;
                            }
                        }
                    }

                    // If a family wasn't found with a home location, use the person's first family
                    if ( family == null )
                    {
                        family = families.FirstOrDefault();
                    }
                }

                // If using a 'Full' view, save the phone numbers and address
                if ( !IsSimple )
                {
                    SetPhoneNumber( rockContext, person, pnHome, null, Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid(), changes );
                    SetPhoneNumber( rockContext, person, pnCell, cbSms, Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid(), changes );
                    
                    // if they entered an address, either add a new one or update their existing
                    var location = new LocationService( rockContext ).Get( acAddress.Street1, acAddress.Street2, acAddress.City, acAddress.State, acAddress.PostalCode, acAddress.Country );
                    if ( location != null )
                    {
                        // if they don't currently have a home location, add one for them.
                        if ( homeLocation == null )
                        {
                            homeLocation = new GroupLocation();
                            homeLocation.GroupLocationTypeValueId = _homeAddressType.Id;
                            homeLocation.Location = location;

                            family.GroupLocations.Add( homeLocation );

                            History.EvaluateChange( familyChanges, "Home Location", string.Empty, location.ToString() );
                        }
                        // otherwise, if the location they entered isn't already their home location
                        else if( homeLocation.Location.Id != location.Id )
                        {
                            // make their current home location a previous location
                            GroupLocation previousLocation = new GroupLocation();
                            previousLocation.GroupLocationTypeValueId = _previousAddressType.Id;
                            previousLocation.Location = homeLocation.Location;
                            family.GroupLocations.Add( previousLocation );

                            // and update their home location to this new one.
                            homeLocation.Location = location;

                            History.EvaluateChange( familyChanges, "Home Location", previousLocation.Location.ToString( ), homeLocation.Location.ToString() );
                        }
                    }

                    // Check for the spouse
                    if ( IsFullWithSpouse && !string.IsNullOrWhiteSpace(tbSpouseFirstName.Text) && !string.IsNullOrWhiteSpace(tbSpouseLastName.Text) )
                    {
                        // try to get their spouse
                        spouse = person.GetSpouse( rockContext );

                        // if they don't have one, we'll create them
                        if ( spouse == null )
                        {
                            spouse = new Person();
                            
                            spouse.ConnectionStatusValueId = _dvcConnectionStatus.Id;
                            spouse.RecordStatusValueId = _dvcRecordStatus.Id;
                            spouse.Gender = Gender.Unknown;

                            spouse.IsEmailActive = true;
                            spouse.EmailPreference = EmailPreference.EmailAllowed;

                            var groupMember = new GroupMember();
                            groupMember.GroupRoleId = _adultRole.Id;
                            groupMember.Person = spouse;

                            family.Members.Add( groupMember );

                            spouse.MaritalStatusValueId = _married.Id;
                            person.MaritalStatusValueId = _married.Id;
                        }

                        // now either way, update their name, email and phone
                        History.EvaluateChange( spouseChanges, "First Name", spouse.FirstName, tbSpouseFirstName.Text.FixCase() );
                        spouse.FirstName = tbSpouseFirstName.Text.FixCase();
                        
                        History.EvaluateChange( spouseChanges, "Last Name", spouse.LastName, tbSpouseLastName.Text.FixCase() );
                        spouse.LastName = tbSpouseLastName.Text.FixCase();

                        if( string.IsNullOrWhiteSpace( tbSpouseEmail.Text ) == false )
                        {
                            History.EvaluateChange( spouseChanges, "Email", spouse.Email, tbSpouseEmail.Text );
                            spouse.Email = tbSpouseEmail.Text;
                        }

                        SetPhoneNumber( rockContext, spouse, pnHome, null, Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid(), spouseChanges );
                        SetPhoneNumber( rockContext, spouse, pnSpouseCell, cbSpouseSms, Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid(), spouseChanges );
                    }
                }

                // Save the person/spouse and change history 
                rockContext.SaveChanges();
                HistoryService.SaveChanges( rockContext, typeof( Person ),
                    Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(), person.Id, changes );
                HistoryService.SaveChanges( rockContext, typeof( Person ),
                    Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(), person.Id, familyChanges );

                if ( spouse != null )
                {
                    HistoryService.SaveChanges( rockContext, typeof( Person ),
                        Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(), spouse.Id, spouseChanges );

                    HistoryService.SaveChanges( rockContext, typeof( Person ),
                        Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(), spouse.Id, familyChanges );
                }

                // now, it's time to either add them to the group, or kick off the Alert Re-Route workflow
                // (Or nothing if there's no problem but they're already in the group)
                GroupMember primaryGroupMember = PersonToGroupMember( rockContext, person );

                GroupMember spouseGroupMember = null;
                if( spouse != null )
                {
                    spouseGroupMember = PersonToGroupMember( rockContext, spouse );
                }

                // prep the workflow service
                var workflowTypeService = new WorkflowTypeService( rockContext );

                bool addToGroup = true;

                // First, check to see if an alert re-route workflow should be launched
                Guid? workflowTypeGuid = GetAttributeValue( "AlertNoteRe-RouteWorkflow" ).AsGuidOrNull();
                if ( workflowTypeGuid.HasValue )
                {
                    // It does, so see if we need to launch it instead
                    WorkflowType alertRerouteWorkflowType = workflowTypeService.Get( workflowTypeGuid.Value );

                    // do either of the people registering have alert notes?
                    int alertNoteCount = new NoteService( rockContext ).Queryable( ).Where( n => (n.EntityId == person.Id) && n.IsAlert == true ).Count( );
                    
                    if( spouse != null )
                    {
                        alertNoteCount += new NoteService( rockContext ).Queryable().Where( n => ( n.EntityId == spouse.Id ) && n.IsAlert == true ).Count();
                    }

                    if( alertNoteCount > 0 )
                    {
                        // yes they do. so first, flag that we should NOT put them in the group
                        addToGroup = false;

                        // and kick off the re-route workflow so security can review.
                        LaunchWorkflow( rockContext, alertRerouteWorkflowType, primaryGroupMember );

                        if( spouseGroupMember != null )
                        {
                            LaunchWorkflow( rockContext, alertRerouteWorkflowType, spouseGroupMember );
                        }
                    }
                }
                
                // if above, we didn't flag that they should not join the group, let's add them
                if ( addToGroup == true )
                {
                    // try to add them to the group (would only fail if the're already in it)
                    TryAddGroupMemberToGroup( rockContext, primaryGroupMember );

                    if ( spouseGroupMember != null )
                    {
                        TryAddGroupMemberToGroup( rockContext, spouseGroupMember );
                    }

                    // is there a workflow to fire off?   
                    workflowTypeGuid = GetAttributeValue( "Workflow" ).AsGuidOrNull();
                    if ( workflowTypeGuid.HasValue )
                    {
                        WorkflowType workflowType = workflowTypeService.Get( workflowTypeGuid.Value );
                        LaunchWorkflow( rockContext, workflowType, primaryGroupMember );

                        if( spouseGroupMember != null )
                        {
                            LaunchWorkflow( rockContext, workflowType, spouseGroupMember );
                        }
                    }
                }


                // Now do a PostBack so they see a 'Completion' page result.
                // Show lava content
                var mergeFields = new Dictionary<string, object>();
                mergeFields.Add( "Group", _group );

                bool showDebug = UserCanEdit && GetAttributeValue( "EnableDebug" ).AsBoolean();
                lResultDebug.Visible = showDebug;
                if ( showDebug )
                {
                    lResultDebug.Text = mergeFields.lavaDebugInfo( _rockContext );
                }

                string template = GetAttributeValue( "ResultLavaTemplate" );
                lResult.Text = template.ResolveMergeFields( mergeFields );

                // Will only redirect if a value is specifed
                NavigateToLinkedPage( "ResultPage" );

                // Show the results
                pnlView.Visible = false;
                pnlResult.Visible = true;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Shows the details.
        /// </summary>
        private void ShowDetails()
        {
            _rockContext = _rockContext ?? new RockContext();

            if ( _group != null )
            {
                // Show lava content
                var mergeFields = new Dictionary<string, object>();
                mergeFields.Add( "Group", _group );

                bool showDebug = UserCanEdit && GetAttributeValue( "EnableDebug" ).AsBoolean();
                lLavaOutputDebug.Visible = showDebug;
                if ( showDebug )
                {
                    lLavaOutputDebug.Text = mergeFields.lavaDebugInfo( _rockContext );
                }

                string template = GetAttributeValue( "LavaTemplate" );
                lLavaOverview.Text = template.ResolveMergeFields( mergeFields );

                // Set visibility based on selected mode
                if ( IsFullWithSpouse )
                {
                    pnlCol1.RemoveCssClass( "col-md-12" ).AddCssClass( "col-md-6" );
                }
                else
                {
                    pnlCol1.RemoveCssClass( "col-md-6" ).AddCssClass( "col-md-12" );
                }
                pnlCol2.Visible = IsFullWithSpouse;

                pnlHomePhone.Visible = !IsSimple;
                pnlCellPhone.Visible = !IsSimple;
                acAddress.Visible = !IsSimple;

                if ( CurrentPersonId.HasValue && _autoFill )
                {
                    var personService = new PersonService( _rockContext );
                    Person person = personService
                        .Queryable( "PhoneNumbers.NumberTypeValue" ).AsNoTracking()
                        .FirstOrDefault( p => p.Id == CurrentPersonId.Value );

                    tbFirstName.Text = CurrentPerson.FirstName;
                    tbLastName.Text = CurrentPerson.LastName;
                    tbEmail.Text = CurrentPerson.Email;

                    if ( !IsSimple )
                    {
                        Guid homePhoneType = Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME.AsGuid();
                        var homePhone = person.PhoneNumbers
                            .FirstOrDefault( n => n.NumberTypeValue.Guid.Equals( homePhoneType ) );
                        if ( homePhone != null )
                        {
                            pnHome.Text = homePhone.NumberFormatted;
                        }

                        Guid cellPhoneType = Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE.AsGuid();
                        var cellPhone = person.PhoneNumbers
                            .FirstOrDefault( n => n.NumberTypeValue.Guid.Equals( cellPhoneType ) );
                        if ( cellPhone != null )
                        {
                            pnCell.Text = cellPhone.NumberFormatted;
                            cbSms.Checked = cellPhone.IsMessagingEnabled;
                        }

                        var homeAddress = person.GetHomeLocation( _rockContext );
                        if ( homeAddress != null )
                        {
                            acAddress.SetValues( homeAddress );
                        }

                        if ( IsFullWithSpouse )
                        {
                            var spouse = person.GetSpouse( _rockContext );
                            if ( spouse != null )
                            {
                                tbSpouseFirstName.Text = spouse.FirstName;
                                tbSpouseLastName.Text = spouse.LastName;
                                tbSpouseEmail.Text = spouse.Email;

                                var spouseCellPhone = spouse.PhoneNumbers
                                    .FirstOrDefault( n => n.NumberTypeValue.Guid.Equals( cellPhoneType ) );
                                if ( spouseCellPhone != null )
                                {
                                    pnSpouseCell.Text = spouseCellPhone.NumberFormatted;
                                    cbSpouseSms.Checked = spouseCellPhone.IsMessagingEnabled;
                                }
                            }
                        }
                    }
                }
            }
        }

        private GroupMember PersonToGroupMember( RockContext rockContext, Person person )
        {
            // puts a person into a group member object, so that we can pass it to a workflow
            GroupMember newGroupMember = new GroupMember();
            newGroupMember.PersonId = person.Id;
            newGroupMember.GroupRoleId = _defaultGroupRole.Id;
            newGroupMember.GroupMemberStatus = (GroupMemberStatus)GetAttributeValue("GroupMemberStatus").AsInteger();
            newGroupMember.GroupId = _group.Id;

            return newGroupMember;
        }

        /// <summary>
        /// Adds the group member to the group if they aren't already in it
        /// </summary>
        private void TryAddGroupMemberToGroup( RockContext rockContext, GroupMember newGroupMember )
        {
            if ( !_group.Members.Any( m => 
                                      m.PersonId == newGroupMember.PersonId &&
                                      m.GroupRoleId == _defaultGroupRole.Id) )
            {
                var groupMemberService = new GroupMemberService(rockContext);
                groupMemberService.Add( newGroupMember );
                    
                rockContext.SaveChanges();
            }
        }

        private void LaunchWorkflow( RockContext rockContext, WorkflowType workflowType, GroupMember groupMember )
        {
            try
            {
                List<string> workflowErrors;
                var workflow = Workflow.Activate( workflowType, workflowType.Name );
                new WorkflowService( rockContext ).Process( workflow, groupMember, out workflowErrors );
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException( ex, this.Context );
            }
        }

        /// <summary>
        /// Checks the settings.
        /// </summary>
        /// <returns></returns>
        private bool CheckSettings()
        {
            _rockContext = _rockContext ?? new RockContext();

            _mode = GetAttributeValue( "Mode" );

            _autoFill = GetAttributeValue( "AutoFillForm" ).AsBoolean();

            tbEmail.Required = _autoFill;

            string registerButtonText = GetAttributeValue( "RegisterButtonAltText" );
            if ( string.IsNullOrWhiteSpace( registerButtonText ) )
            {
                registerButtonText = "Register";
            }
            btnRegister.Text = registerButtonText;

            var groupService = new GroupService( _rockContext );

            Guid? groupGuid = GetAttributeValue( "Group" ).AsGuidOrNull();
            if ( groupGuid.HasValue )
            {
                _group = groupService.Get( groupGuid.Value );
            }

            if ( _group == null )
            {
                groupGuid = PageParameter( "GroupGuid" ).AsGuidOrNull();
                if ( groupGuid.HasValue )
                {
                    _group = groupService.Get( groupGuid.Value );
                }
            }

            if ( _group == null )
            {
                int? groupId = PageParameter( "GroupId" ).AsIntegerOrNull();
                if ( groupId.HasValue )
                {
                    _group = groupService.Get( groupId.Value );
                }
            }

            if ( _group == null )
            {
                nbNotice.Heading = "Unknown Group";
                nbNotice.Text = "<p>This page requires a valid group id parameter, and there was not one provided.</p>";
                return false;
            }
            else
            {
                _defaultGroupRole = _group.GroupType.DefaultGroupRole;
            }

            _dvcConnectionStatus = DefinedValueCache.Read( GetAttributeValue( "ConnectionStatus" ).AsGuid() );
            if ( _dvcConnectionStatus == null )
            {
                nbNotice.Heading = "Invalid Connection Status";
                nbNotice.Text = "<p>The selected Connection Status setting does not exist.</p>";
                return false;
            }

            _dvcRecordStatus = DefinedValueCache.Read( GetAttributeValue( "RecordStatus" ).AsGuid() );
            if ( _dvcRecordStatus == null )
            {
                nbNotice.Heading = "Invalid Record Status";
                nbNotice.Text = "<p>The selected Record Status setting does not exist.</p>";
                return false;
            }

            _married = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_MARRIED.AsGuid() );
            _homeAddressType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME.AsGuid() );
            _previousAddressType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_PREVIOUS.AsGuid() );
            _familyType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
            _adultRole = _familyType.Roles.FirstOrDefault( r => r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ) );

            if ( _married == null || _homeAddressType == null || _familyType == null || _adultRole == null || _previousAddressType == null )
            {
                nbNotice.Heading = "Missing System Value";
                nbNotice.Text = "<p>There is a missing or invalid system value. Check the settings for Marital Status of 'Married', Location Type of 'Home' and 'Previous', Group Type of 'Family', and Family Group Role of 'Adult'.</p>";
                return false;
            }

            return true;

        }

        /// <summary>
        /// Sets the phone number.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="person">The person.</param>
        /// <param name="pnbNumber">The PNB number.</param>
        /// <param name="cbSms">The cb SMS.</param>
        /// <param name="phoneTypeGuid">The phone type unique identifier.</param>
        /// <param name="changes">The changes.</param>
        private void SetPhoneNumber( RockContext rockContext, Person person, PhoneNumberBox pnbNumber, RockCheckBox cbSms, Guid phoneTypeGuid, List<string> changes )
        {
            var phoneType = DefinedValueCache.Read( phoneTypeGuid );
            if ( phoneType != null )
            {
                var phoneNumber = person.PhoneNumbers.FirstOrDefault( n => n.NumberTypeValueId == phoneType.Id );
                string oldPhoneNumber = string.Empty;
                if ( phoneNumber == null )
                {
                    phoneNumber = new PhoneNumber { NumberTypeValueId = phoneType.Id };
                }
                else
                {
                    oldPhoneNumber = phoneNumber.NumberFormattedWithCountryCode;
                }
                
                // if they put a valid phone number, update / add what they have on record.
                // If they left it blank, don't do anything. It's possible they have a number in our system and
                // just didn't bother typing it in again.
                string cleanNumber = PhoneNumber.CleanNumber( pnbNumber.Number );
                if ( string.IsNullOrWhiteSpace( cleanNumber ) == false )
                {
                    phoneNumber.CountryCode = PhoneNumber.CleanNumber( pnbNumber.CountryCode );
                    phoneNumber.Number = cleanNumber;

                    if ( phoneNumber.Id <= 0)
                    {
                        person.PhoneNumbers.Add( phoneNumber );
                    }

                    // let them toggle their SMS preference
                    if ( cbSms != null )
                    {
                        phoneNumber.IsMessagingEnabled = cbSms.Checked;
                        person.PhoneNumbers
                            .Where( n => n.NumberTypeValueId != phoneType.Id )
                            .ToList()
                            .ForEach( n => n.IsMessagingEnabled = false );
                    }

                    History.EvaluateChange( changes, string.Format( "{0} Phone", phoneType.Value ), oldPhoneNumber, phoneNumber.NumberFormattedWithCountryCode );
                }
            }
        }

        #endregion
    }
}