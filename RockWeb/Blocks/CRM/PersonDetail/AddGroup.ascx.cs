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
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI.Controls;

namespace RockWeb.Blocks.Crm.PersonDetail
{
    /// <summary>
    /// Block for adding new families
    /// </summary>
    [DisplayName( "Add Group" )]
    [Category( "CRM > Person Detail" )]
    [Description( "Allows for adding a new group and the people in the group (e.g. New Families." )]

    [GroupTypeField( "Group Type", "The group type to display groups for (default is Family)", false, Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY, "", 0 )]
    [GroupField( "Parent Group", "The parent group to add the new group to (default is none)", false, "", "", 1 )]
    [BooleanField( "Show Middle Name", "Show an edit box for Middle Name.", false, order: 2 )]
    [BooleanField( "Enable Common Last Name", "Autofills the last name field when adding a new group member with the last name of the first group member.", true, order: 3 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_CONNECTION_STATUS, "Default Connection Status",
        "The connection status that should be set by default", false, false, Rock.SystemGuid.DefinedValue.PERSON_CONNECTION_STATUS_VISITOR, "", 4 )]
    [BooleanField( "Gender", "Require a gender for each person", "Don't require", "Should Gender be required for each person added?", false, "", 5 )]
    [BooleanField( "Birth Date", "Require a birthdate for each person", "Don't require", "Should a Birthdate be required for each person added?", false, "", 6 )]
    [BooleanField( "Grade", "Require a grade for each child", "Don't require", "When Family group type, should Grade be required for each child added?", false, "", 7 )]
    [BooleanField( "Show Inactive Campuses", "Determines if inactive campuses should be shown.", true, order: 8 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_MARITAL_STATUS, "Adult Marital Status", "When Family group type, the default marital status for adults in the family.", false, false, "", "", 9 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.PERSON_MARITAL_STATUS, "Child Marital Status", "When Famiy group type, the marital status to use for children in the family.", false, false,
        Rock.SystemGuid.DefinedValue.PERSON_MARITAL_STATUS_SINGLE, "", 10 )]
    [BooleanField( "Marital Status Confirmation", "When Family group type, should user be asked to confirm saving an adult without a marital status?", true, "", 11 )]
    [CustomDropdownListField( "Address", "Should an address be required for the family?", "REQUIRE^Require an address,HOMELESS^Require an address unless family is homeless,NOTREQUIRED^Don't require", false, "NOTREQUIRED", "", 12 )]
    [DefinedValueField( Rock.SystemGuid.DefinedType.GROUP_LOCATION_TYPE, "Location Type",
        "The type of location that address should use", false, false, Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME, "", 13 )]
    [BooleanField( "Show Cell Phone Number First", "Should the cell phone number be listed first before home phone number?", false, "", 14 )]
    [BooleanField( "Phone Number", "Require a phone number", "Don't require", "Should a phone number be required for at least one person?", false, "", 15 )]
    [BooleanField( "SMS", "SMS is enabled by default", "SMS is not enabled by default", "Should SMS be enabled for cell phone numbers by default?", false, "", 16 )]
    [AttributeCategoryField( "Attribute Categories", "The Person Attribute Categories to display attributes from", true, "Rock.Model.Person", false, "", "", 17 )]
    public partial class AddGroup : Rock.Web.UI.RockBlock
    {
        #region Rock Controls

        protected global::Rock.Web.UI.Controls.NotificationBox nbValidation;
        protected global::Rock.Web.UI.Controls.RockTextBox tbGroupName;
        protected global::Rock.Web.UI.Controls.NewGroupMembers nfmMembers;
        protected global::Rock.Web.UI.Controls.CampusPicker cpCampus;
        protected global::Rock.Web.UI.Controls.RockDropDownList ddlMaritalStatus;
        protected global::Rock.Web.UI.Controls.AddressControl acAddress;
        protected global::Rock.Web.UI.Controls.RockCheckBox cbHomeless;
        protected global::Rock.Web.UI.Controls.NewGroupContactInfo nfciContactInfo;
        protected global::Rock.Web.UI.Controls.NotificationBox nbMessages;

        #endregion

        #region Fields

        private GroupTypeCache _groupType = null;
        private bool _isFamilyGroupType = false;
        protected string _groupTypeName = string.Empty;

        private DefinedValueCache _locationType = null;

        private bool _confirmMaritalStatus = true;
        private int _childRoleId = 0;
        private List<NewGroupAttributes> attributeControls = new List<NewGroupAttributes>();
        private Dictionary<string, int?> _verifiedLocations = null;
        private DefinedValueCache _homePhone = null;
        private DefinedValueCache _cellPhone = null;
        private bool _smsEnabled = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the index of the current category.
        /// </summary>
        /// <value>
        /// The index of the current category.
        /// </value>
        protected int CurrentPageIndex { get; set; }

        /// <summary>
        /// Gets or sets the group members that have been added by user
        /// </summary>
        /// <value>
        /// The group members.
        /// </value>
        protected List<GroupMember> GroupMembers { get; set; }

        /// <summary>
        /// Gets or sets any possible duplicates for each group member
        /// </summary>
        /// <value>
        /// The duplicates.
        /// </value>
        protected Dictionary<Guid, List<Person>> Duplicates { get; set; }

        /// <summary>
        /// Gets or sets any possible matching addresses for each group member
        /// </summary>
        /// <value>
        /// The people with matching addresses.
        /// </value>
        protected Dictionary<Guid, List<Person>> MatchingAddresses { get; set; }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Restores the view-state information from a previous user control request that was saved by the <see cref="M:System.Web.UI.UserControl.SaveViewState" /> method.
        /// </summary>
        /// <param name="savedState">An <see cref="T:System.Object" /> that represents the user control state to be restored.</param>
        protected override void LoadViewState( object savedState )
        {
            base.LoadViewState( savedState );

            CurrentPageIndex = ViewState["CurrentPageIndex"] as int? ?? 0;

            string json = ViewState["GroupMembers"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                GroupMembers = new List<GroupMember>();
            }
            else
            {
                GroupMembers = JsonConvert.DeserializeObject<List<GroupMember>>( json );
            }

            json = ViewState["Duplicates"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                Duplicates = new Dictionary<Guid, List<Person>>();
            }
            else
            {
                Duplicates = JsonConvert.DeserializeObject<Dictionary<Guid, List<Person>>>( json );
            }

            json = ViewState["MatchingAddresses"] as string;
            if ( string.IsNullOrWhiteSpace( json ) )
            {
                MatchingAddresses = new Dictionary<Guid, List<Person>>();
            }
            else
            {
                MatchingAddresses = JsonConvert.DeserializeObject<Dictionary<Guid, List<Person>>>( json );
            }

            _verifiedLocations = ViewState["VerifiedLocations"] as Dictionary<string, int?>;

            CreateControls( false );
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            // Tell the browsers to not cache. This will help prevent browser using a stale person guid when the user uses the Back button
            Page.Response.Cache.SetCacheability( System.Web.HttpCacheability.NoCache );
            Page.Response.Cache.SetExpires( DateTime.UtcNow.AddHours( -1 ) );
            Page.Response.Cache.SetNoStore();

            _groupType = GroupTypeCache.Read( GetAttributeValue( "GroupType" ).AsGuid() );
            if ( _groupType == null )
            {
                _groupType = GroupTypeCache.GetFamilyGroupType();
            }

            _groupTypeName = _groupType.Name;
            _isFamilyGroupType = _groupType.Guid.Equals( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
            _locationType = _groupType.LocationTypeValues.FirstOrDefault( v => v.Guid.Equals( GetAttributeValue( "LocationType" ).AsGuid() ) );
            if ( _locationType == null )
            {
                _locationType = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME );
            }

            if ( _isFamilyGroupType )
            {
                divGroupName.Visible = false;
                var campusi = GetAttributeValue( "ShowInactiveCampuses" ).AsBoolean() ? CampusCache.All() : CampusCache.All().Where( c => c.IsActive == true ).ToList();
                cpCampus.Campuses = campusi;
                cpCampus.Visible = campusi.Any();
                if ( campusi.Count == 1 )
                {
                    cpCampus.SelectedCampusId = campusi.FirstOrDefault().Id;
                }

                ddlMaritalStatus.Visible = true;
                ddlMaritalStatus.BindToDefinedType( DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.PERSON_MARITAL_STATUS.AsGuid() ), true );
                var adultMaritalStatus = DefinedValueCache.Read( GetAttributeValue( "AdultMaritalStatus" ).AsGuid() );
                if ( adultMaritalStatus != null )
                {
                    ddlMaritalStatus.SetValue( adultMaritalStatus.Id );
                }

                _childRoleId = _groupType.Roles
                    .Where( r => r.Guid.Equals( Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ) )
                    .Select( r => r.Id )
                    .FirstOrDefault();
            }
            else
            {
                divGroupName.Visible = true;
                tbGroupName.Label = _groupTypeName + " Name";
                cpCampus.Visible = false;
                ddlMaritalStatus.Visible = false;
            }

            nfmMembers.ShowGrade = _isFamilyGroupType;
            nfmMembers.RequireGender = GetAttributeValue( "Gender" ).AsBoolean();
            nfmMembers.RequireBirthdate = GetAttributeValue( "BirthDate" ).AsBoolean();
            nfmMembers.RequireGrade = GetAttributeValue( "Grade" ).AsBoolean();
            _smsEnabled = GetAttributeValue( "SMS" ).AsBoolean();

            lTitle.Text = string.Format( "Add {0}", _groupType.Name ).FormatAsHtmlTitle();

            nfciContactInfo.ShowCellPhoneFirst = GetAttributeValue( "ShowCellPhoneNumberFirst" ).AsBoolean();

            acAddress.Required = GetAttributeValue( "Address" ) == "REQUIRED";
            cbHomeless.Visible = GetAttributeValue( "Address" ) == "HOMELESS";

            _homePhone = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_HOME );
            _cellPhone = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_PHONE_TYPE_MOBILE );


            _confirmMaritalStatus = _isFamilyGroupType && GetAttributeValue( "MaritalStatusConfirmation" ).AsBoolean();
            if ( _confirmMaritalStatus )
            {
                string script = string.Format(
      @"$('a.js-confirm-marital-status').click(function( e ){{
        var anyAdults = false;
        $(""input[id$='_rblRole_0']"").each(function() {{
            if ( $(this).prop('checked') ) {{
                anyAdults = true;
            }}
        }});

        if ( anyAdults ) {{
            if ( $('#{0}').val() == '' ) {{
                e.preventDefault();
                Rock.dialogs.confirm('You have not selected a marital status for the adults in this new family. Are you sure you want to continue?', function (result) {{
                    if (result) {{
                        window.location = e.target.href ? e.target.href : e.target.parentElement.href;
                    }}
                }});
            }}
            }}
      }});",
      ddlMaritalStatus.ClientID );

                ScriptManager.RegisterStartupScript( btnNext, btnNext.GetType(), "confirm-marital-status", script, true );
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            nbValidation.Visible = false;

            if ( !Page.IsPostBack )
            {
                GroupMembers = new List<GroupMember>();
                Duplicates = new Dictionary<Guid, List<Person>>();
                MatchingAddresses = new Dictionary<Guid, List<Person>>();
                _verifiedLocations = new Dictionary<string, int?>();
                AddGroupMember();
                CreateControls( true );
            }
            else
            {
                GetControlData();
            }
        }

        /// <summary>
        /// Saves any user control view-state changes that have occurred since the last page postback.
        /// </summary>
        /// <returns>
        /// Returns the user control's current view state. If there is no view state associated with the control, it returns null.
        /// </returns>
        protected override object SaveViewState()
        {
            var jsonSetting = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new Rock.Utility.IgnoreUrlEncodedKeyContractResolver()
            };

            ViewState["CurrentPageIndex"] = CurrentPageIndex;
            ViewState["GroupMembers"] = JsonConvert.SerializeObject( GroupMembers, Formatting.None, jsonSetting );
            ViewState["Duplicates"] = JsonConvert.SerializeObject( Duplicates, Formatting.None, jsonSetting );
            ViewState["MatchingAddresses"] = JsonConvert.SerializeObject( MatchingAddresses, Formatting.None, jsonSetting );
            ViewState["VerifiedLocations"] = _verifiedLocations;

            return base.SaveViewState();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender( EventArgs e )
        {
            if ( _isFamilyGroupType )
            {
                var adults = GroupMembers.Where( m => m.GroupRoleId != _childRoleId ).ToList();
                ddlMaritalStatus.Visible = adults.Any();
            }

            base.OnPreRender( e );
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the AddGroupMemberClick event of the nfmMembers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void nfmMembers_AddGroupMemberClick( object sender, EventArgs e )
        {
            AddGroupMember();
            CreateControls( true );
        }

        /// <summary>
        /// Handles the RoleUpdated event of the groupMemberRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected void groupMemberRow_RoleUpdated( object sender, EventArgs e )
        {
            NewGroupMembersRow row = sender as NewGroupMembersRow;
            row.ShowGradePicker = row.RoleId == _childRoleId;
            row.ShowGradeColumn = _isFamilyGroupType;
        }

        /// <summary>
        /// Handles the DeleteClick event of the groupMemberRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void groupMemberRow_DeleteClick( object sender, EventArgs e )
        {
            NewGroupMembersRow row = sender as NewGroupMembersRow;
            var groupMember = GroupMembers.FirstOrDefault( m => m.Person.Guid.Equals( row.PersonGuid ) );
            if ( groupMember != null )
            {
                GroupMembers.Remove( groupMember );
            }

            CreateControls( true );
        }

        /// <summary>
        /// Handles the Click event of the btnPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnPrevious_Click( object sender, EventArgs e )
        {
            if ( CurrentPageIndex > 0 )
            {
                CurrentPageIndex--;
                CreateControls( true );
            }
        }

        /// <summary>
        /// Handles the Click event of the btnNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void btnNext_Click( object sender, EventArgs e )
        {

            if ( Page.IsValid )
            {
                var errorMessages = new List<string>();

                var people = GroupMembers.Where( m => m.Person != null ).Select( m => m.Person ).ToList();
                var children = GroupMembers.Where( m => m.GroupRoleId == _childRoleId ).Select( m => m.Person ).ToList();

                if ( CurrentPageIndex == 0 )
                {
                    if ( GetAttributeValue( "Gender" ).AsBoolean() && people.Any( p => p.Gender == Gender.Unknown ) )
                    {
                        errorMessages.Add( "Gender is required for all members." );
                    }
                    if ( GetAttributeValue( "BirthDate" ).AsBoolean() && people.Any( p => !p.BirthDate.HasValue ) )
                    {
                        errorMessages.Add( "Birthdate is required for all members." );
                    }
                    if ( GetAttributeValue( "Grade" ).AsBoolean() && children.Any( p => !p.GraduationYear.HasValue ) )
                    {
                        errorMessages.Add( "Grade is required for all children." );
                    }

                    int? locationId = null;
                    string locationKey = GetLocationKey();
                    if ( !string.IsNullOrEmpty( locationKey ) )
                    {
                        if ( _verifiedLocations.ContainsKey( locationKey ) )
                        {
                            locationId = _verifiedLocations[locationKey];
                        }
                        else
                        {
                            using ( var rockContext = new RockContext() )
                            {
                                var location = new LocationService( rockContext ).Get( acAddress.Street1, acAddress.Street2, acAddress.City, acAddress.State, acAddress.PostalCode, acAddress.Country );
                                locationId = location != null ? location.Id : (int?)null;
                                _verifiedLocations.AddOrIgnore( locationKey, locationId );
                            }
                        }
                    }

                    if ( !locationId.HasValue )
                    {
                        string addressRequired = GetAttributeValue( "Address" );
                        if ( addressRequired == "REQUIRED" )
                        {
                            errorMessages.Add( "Address is required." );
                        }

                        if ( addressRequired == "HOMELESS" && !cbHomeless.Checked )
                        {
                            errorMessages.Add( "Address is required unless the family is homeless." );
                        }
                    }
                }

                if ( CurrentPageIndex == 1 )
                {
                    if ( GetAttributeValue( "PhoneNumber" ).AsBoolean() && !people.Any( p => p.PhoneNumbers.Any() ) )
                    {
                        errorMessages.Add( "At least one phone number is required." );
                    }
                }

                if ( errorMessages.Any() )
                {
                    nbValidation.Text = string.Format( "<ul><li>{0}</li></ul>", errorMessages.AsDelimited( "</li><li>" ) );
                    nbValidation.Visible = true;
                }
                else
                {
                    if ( CurrentPageIndex < ( attributeControls.Count + 1 ) )
                    {
                        CurrentPageIndex++;
                        CreateControls( true );
                    }
                    else
                    {
                        if ( GroupMembers.Any() )
                        {
                            if ( CurrentPageIndex == ( attributeControls.Count + 1 ) && ( FindDuplicates() | FindMatchingAddresses() ) )
                            {
                                CurrentPageIndex++;
                                CreateControls( true );
                            }
                            else
                            {
                                var rockContext = new RockContext();

                                Guid? parentGroupGuid = GetAttributeValue( "ParentGroup" ).AsGuidOrNull();

                                rockContext.WrapTransaction( () =>
                                {
                                    Group group = null;
                                    if ( _isFamilyGroupType )
                                    {
                                        group = GroupService.SaveNewFamily( rockContext, GroupMembers, cpCampus.SelectedValueAsInt(), true );
                                    }
                                    else
                                    {
                                        group = GroupService.SaveNewGroup( rockContext, _groupType.Id, parentGroupGuid, tbGroupName.Text, GroupMembers, null, true );
                                    }

                                    if ( group != null )
                                    {
                                        string locationKey = GetLocationKey();
                                        if ( !string.IsNullOrEmpty( locationKey ) && _verifiedLocations.ContainsKey( locationKey ) )
                                        {
                                            GroupService.AddNewGroupAddress( rockContext, group, _locationType.Guid.ToString(), _verifiedLocations[locationKey] );
                                        }
                                    }
                                } );

                                Response.Redirect( string.Format( "~/Person/{0}", GroupMembers[0].Person.Id ), false );
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the controls.
        /// </summary>
        /// <param name="setSelection">if set to <c>true</c> [set selection].</param>
        private void CreateControls( bool setSelection )
        {
            // Load all the attribute controls
            attributeControls.Clear();
            pnlAttributes.Controls.Clear();
            phDuplicates.Controls.Clear();
            phMatchingAddresses.Controls.Clear();

            var rockContext = new RockContext();
            var attributeService = new AttributeService( rockContext );
            var locationService = new LocationService( rockContext );

            foreach ( string categoryGuid in GetAttributeValue( "AttributeCategories" ).SplitDelimitedValues( false ) )
            {
                Guid guid = Guid.Empty;
                if ( Guid.TryParse( categoryGuid, out guid ) )
                {
                    var category = CategoryCache.Read( guid );
                    if ( category != null )
                    {
                        var attributeControl = new NewGroupAttributes();
                        attributeControl.ClearRows();
                        pnlAttributes.Controls.Add( attributeControl );
                        attributeControls.Add( attributeControl );
                        attributeControl.ID = "groupAttributes_" + category.Id.ToString();
                        attributeControl.CategoryId = category.Id;

                        foreach ( var attribute in attributeService.GetByCategoryId( category.Id ) )
                        {
                            if ( attribute.IsAuthorized( Authorization.EDIT, CurrentPerson ) )
                            {
                                attributeControl.AttributeList.Add( AttributeCache.Read( attribute ) );
                            }
                        }
                    }
                }
            }

            nfmMembers.ClearRows();
            nfciContactInfo.ClearRows();

            var groupMemberService = new GroupMemberService( rockContext );
            int defaultRoleId = _groupType.DefaultGroupRoleId ?? _groupType.Roles.Select( r => r.Id ).FirstOrDefault();

            var location = new Location();
            acAddress.GetValues( location );

            foreach ( var groupMember in GroupMembers )
            {
                string groupMemberGuidString = groupMember.Person.Guid.ToString().Replace( "-", "_" );

                var groupMemberRow = new NewGroupMembersRow();
                groupMemberRow.GroupTypeId = _groupType.Id;
                nfmMembers.Controls.Add( groupMemberRow );
                groupMemberRow.ID = string.Format( "row_{0}", groupMemberGuidString );
                groupMemberRow.RoleUpdated += groupMemberRow_RoleUpdated;
                groupMemberRow.DeleteClick += groupMemberRow_DeleteClick;
                groupMemberRow.PersonGuid = groupMember.Person.Guid;
                groupMemberRow.RequireGender = nfmMembers.RequireGender;
                groupMemberRow.RequireBirthdate = nfmMembers.RequireBirthdate;
                groupMemberRow.RequireGrade = nfmMembers.RequireGrade;
                groupMemberRow.RoleId = groupMember.GroupRoleId;
                groupMemberRow.ShowGradeColumn = _isFamilyGroupType;
                groupMemberRow.ShowGradePicker = groupMember.GroupRoleId == _childRoleId;
                groupMemberRow.ShowMiddleName = this.GetAttributeValue( "ShowMiddleName" ).AsBoolean();
                groupMemberRow.ValidationGroup = BlockValidationGroup;

                var contactInfoRow = new NewGroupContactInfoRow();
                nfciContactInfo.Controls.Add( contactInfoRow );
                contactInfoRow.ID = string.Format( "ci_row_{0}", groupMemberGuidString );
                contactInfoRow.PersonGuid = groupMember.Person.Guid;
                contactInfoRow.IsMessagingEnabled = _smsEnabled;
                contactInfoRow.PersonName = groupMember.Person.FullName;
                contactInfoRow.ShowCellPhoneFirst = this.GetAttributeValue( "ShowCellPhoneNumberFirst" ).AsBoolean();

                if ( _homePhone != null )
                {
                    var homePhoneNumber = groupMember.Person.PhoneNumbers.Where( p => p.NumberTypeValueId == _homePhone.Id ).FirstOrDefault();
                    if ( homePhoneNumber != null )
                    {
                        contactInfoRow.HomePhoneNumber = PhoneNumber.FormattedNumber( homePhoneNumber.CountryCode, homePhoneNumber.Number );
                        contactInfoRow.HomePhoneCountryCode = homePhoneNumber.CountryCode;
                    }
                    else
                    {
                        contactInfoRow.HomePhoneNumber = string.Empty;
                        contactInfoRow.HomePhoneCountryCode = string.Empty;
                    }
                }

                if ( _cellPhone != null )
                {
                    var cellPhoneNumber = groupMember.Person.PhoneNumbers.Where( p => p.NumberTypeValueId == _cellPhone.Id ).FirstOrDefault();
                    if ( cellPhoneNumber != null )
                    {
                        contactInfoRow.CellPhoneNumber = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellPhoneNumber.Number );
                        contactInfoRow.CellPhoneCountryCode = cellPhoneNumber.CountryCode;
                    }
                    else
                    {
                        contactInfoRow.CellPhoneNumber = string.Empty;
                        contactInfoRow.CellPhoneCountryCode = string.Empty;
                    }
                }

                contactInfoRow.Email = groupMember.Person.Email;

                if ( setSelection )
                {
                    if ( groupMember.Person != null )
                    {
                        groupMemberRow.TitleValueId = groupMember.Person.TitleValueId;
                        groupMemberRow.FirstName = groupMember.Person.FirstName;
                        groupMemberRow.MiddleName = groupMember.Person.MiddleName;
                        groupMemberRow.LastName = groupMember.Person.LastName;
                        groupMemberRow.SuffixValueId = groupMember.Person.SuffixValueId;
                        groupMemberRow.Gender = groupMember.Person.Gender;
                        groupMemberRow.BirthDate = groupMember.Person.BirthDate;
                        groupMemberRow.ConnectionStatusValueId = groupMember.Person.ConnectionStatusValueId;
                        groupMemberRow.GradeOffset = groupMember.Person.GradeOffset;
                    }
                }

                foreach ( var attributeControl in attributeControls )
                {
                    var attributeRow = new NewGroupAttributesRow();
                    attributeControl.Controls.Add( attributeRow );
                    attributeRow.ID = string.Format( "{0}_{1}", attributeControl.ID, groupMemberGuidString );
                    attributeRow.AttributeList = attributeControl.AttributeList;
                    attributeRow.PersonGuid = groupMember.Person.Guid;
                    attributeRow.PersonName = groupMember.Person.FullName;

                    if ( setSelection )
                    {
                        attributeRow.SetEditValues( groupMember.Person );
                    }
                }

                if ( Duplicates.ContainsKey( groupMember.Person.Guid ) | MatchingAddresses.ContainsKey( groupMember.Person.Guid ) )
                {
                    // matching new record column
                    var dupRow = new HtmlGenericControl( "div" );
                    dupRow.AddCssClass( "row row-duplicate" );
                    dupRow.ID = string.Format( "dupRow_{0}", groupMemberGuidString );
                    phDuplicates.Controls.Add( dupRow );

                    var newPersonCol = new HtmlGenericControl( "div" );
                    newPersonCol.AddCssClass( "col-md-4" );
                    newPersonCol.ID = string.Format( "newPersonCol_{0}", groupMemberGuidString );
                    dupRow.Controls.Add( newPersonCol );

                    newPersonCol.Controls.Add( PersonHtmlPanel(
                        groupMemberGuidString,
                        groupMember.Person,
                        groupMember.GroupRole,
                        location,
                        "newPersonPnl",
                        rockContext ) );

                    LinkButton lbRemoveMember = new LinkButton();
                    lbRemoveMember.ID = string.Format( "lbRemoveMember_{0}", groupMemberGuidString );
                    lbRemoveMember.AddCssClass( "btn btn-danger btn-xs" );
                    lbRemoveMember.Text = "Do Not Add Individual";
                    lbRemoveMember.Click += lbRemoveMember_Click;
                    newPersonCol.Controls.Add( lbRemoveMember );

                    // duplicate records column
                    if ( Duplicates.ContainsKey( groupMember.Person.Guid ) )
                    {
                        var dupPersonCol = new HtmlGenericControl( "div" );
                        dupPersonCol.AddCssClass( "col-md-4" );
                        dupPersonCol.ID = string.Format( "dupPersonCol_{0}", groupMemberGuidString );
                        dupRow.Controls.Add( dupPersonCol );

                        var duplicateHeader = new HtmlGenericControl( "h4" );
                        duplicateHeader.InnerText = "Possible Duplicate Records";
                        dupPersonCol.Controls.Add( duplicateHeader );

                        foreach ( var duplicate in Duplicates[groupMember.Person.Guid] )
                        {
                            GroupTypeRole groupTypeRole = null;
                            Location duplocation = null;

                            var dupGroupMember = groupMemberService.Queryable()
                                .Where( a => a.PersonId == duplicate.Id )
                                .Where( a => a.Group.GroupTypeId == _groupType.Id )
                                .Select( s => new
                                {
                                    s.GroupRole,
                                    GroupLocation = s.Group.GroupLocations.Where( a => a.GroupLocationTypeValue.Guid.Equals( _locationType.Guid ) ).Select( a => a.Location ).FirstOrDefault()
                                } )
                                .FirstOrDefault();
                            if ( dupGroupMember != null )
                            {
                                groupTypeRole = dupGroupMember.GroupRole;
                                duplocation = dupGroupMember.GroupLocation;
                            }

                            dupPersonCol.Controls.Add( PersonHtmlPanel(
                                groupMemberGuidString,
                                duplicate,
                                groupTypeRole,
                                duplocation,
                                "dupPersonPnl",
                                rockContext ) );
                        }
                    }

                    // duplicate addresses
                    if ( MatchingAddresses.ContainsKey( groupMember.Person.Guid ) )
                    {
                        var matchAddressCol = new HtmlGenericControl( "div" );
                        matchAddressCol.AddCssClass( "col-md-4" );
                        matchAddressCol.ID = string.Format( "matchAddressCol_{0}", groupMemberGuidString );
                        dupRow.Controls.Add( matchAddressCol );

                        var matchAddressHeader = new HtmlGenericControl( "h4" );
                        matchAddressHeader.InnerText = "Duplicate Addresses";
                        matchAddressCol.Controls.Add( matchAddressHeader );

                        foreach ( var duplicate in MatchingAddresses[groupMember.Person.Guid] )
                        {
                            GroupTypeRole groupTypeRole = null;
                            Location duplocation = null;

                            var dupGroupMember = groupMemberService.Queryable()
                                .Where( a => a.PersonId == duplicate.Id )
                                .Where( a => a.Group.GroupTypeId == _groupType.Id )
                                .Select( s => new
                                {
                                    s.GroupRole,
                                    GroupLocation = s.Group.GroupLocations.Where( a => a.GroupLocationTypeValue.Guid.Equals( _locationType.Guid ) ).Select( a => a.Location ).FirstOrDefault()
                                } )
                                .FirstOrDefault();
                            if ( dupGroupMember != null )
                            {
                                groupTypeRole = dupGroupMember.GroupRole;
                                duplocation = dupGroupMember.GroupLocation;
                            }

                            matchAddressCol.Controls.Add( PersonHtmlPanel(
                                groupMemberGuidString,
                                duplicate,
                                groupTypeRole,
                                duplocation,
                                "matchAddressPnl",
                                rockContext ) );
                        }
                    }
                }
            }

            ShowPage();
        }

        protected void lbRemoveMember_Click( object sender, EventArgs e )
        {
            Guid personGuid = ( (LinkButton)sender ).ID.Substring( 15 ).Replace( "_", "-" ).AsGuid();
            var groupMember = GroupMembers.Where( f => f.Person.Guid.Equals( personGuid ) ).FirstOrDefault();
            if ( groupMember != null )
            {
                GroupMembers.Remove( groupMember );
                Duplicates.Remove( personGuid );
                MatchingAddresses.Remove( personGuid );
                if ( !GroupMembers.Any() )
                {
                    AddGroupMember();
                    CurrentPageIndex = 0;
                }

                CreateControls( true );
            }
        }

        /// <summary>
        /// Persons the HTML panel.
        /// </summary>
        /// <param name="groupMemberGuidString">The group member unique identifier string.</param>
        /// <param name="person">The person.</param>
        /// <param name="groupTypeRole">The group type role.</param>
        /// <param name="location">The location.</param>
        /// <param name="rockContext">The rock context.</param>
        /// <returns></returns>
        private Panel PersonHtmlPanel(
            string groupMemberGuidString,
            Person person,
            GroupTypeRole groupTypeRole,
            Location location,
            string controlIdPrefix,
            RockContext rockContext )
        {
            var personInfoHtml = new StringBuilder();

            Guid? recordTypeValueGuid = null;
            if ( person.RecordTypeValueId.HasValue )
            {
                recordTypeValueGuid = DefinedValueCache.Read( person.RecordTypeValueId.Value, rockContext ).Guid;
            }

            string personName = string.Format( "{0} <small>(New Record)</small>", person.FullName );
            if ( person.Id > 0 )
            {
                string personUrl = ResolveRockUrl( string.Format( "~/person/{0}", person.Id ) );
                personName = string.Format( "<a href='{0}' target='_blank'>{1}</a>", personUrl, person.FullName );
            }

            personInfoHtml.Append( "<div class='row margin-b-lg'>" );

            // add photo if it's not the new record
            if ( person.Id > 0 )
            {
                personInfoHtml.Append( "<div class='col-md-2'>" );
                if ( person.PhotoId.HasValue )
                {
                    personInfoHtml.AppendFormat(
                        "<img src='{0}' class='img-thumbnail'>",
                        Person.GetPersonPhotoUrl( person, 200, 200 ) );
                }

                personInfoHtml.Append( "</div>" );
            }

            personInfoHtml.Append( "<div class='col-md-10'>" );
            personInfoHtml.AppendFormat( "<h4 class='margin-t-none'>{0}</h4>", personName );

            if ( groupTypeRole != null )
            {
                personInfoHtml.Append( groupTypeRole.Name );
            }

            int? personAge = person.Age;
            if ( personAge.HasValue )
            {
                personInfoHtml.AppendFormat( " <em>({0} yrs old)</em>", personAge.Value );
            }

            var groupMembers = person.GetGroupMembers( _groupType.Id, false, rockContext );
            if ( groupMembers != null && groupMembers.Any() )
            {
                personInfoHtml.AppendFormat( 
                    "<p><strong>{0} Members:</strong> {1}", 
                    _groupType.Name,
                    groupMembers.Select( m => m.Person.NickName ).ToList().AsDelimited( ", " ) );
            }

            if ( location != null && location.GetFullStreetAddress().IsNotNullOrWhitespace() )
            {
                personInfoHtml.AppendFormat( "<p><strong>Address</strong><br/>{0}</p>", location.GetFullStreetAddress().ConvertCrLfToHtmlBr() );
            }

            // Generate the HTML for Email and PhoneNumbers
            if ( !string.IsNullOrWhiteSpace( person.Email ) || person.PhoneNumbers.Any() )
            {
                string emailAndPhoneHtml = "<p class='margin-t-sm'>";
                emailAndPhoneHtml += person.Email;
                string phoneNumberList = string.Empty;
                foreach ( var phoneNumber in person.PhoneNumbers )
                {
                    var phoneType = DefinedValueCache.Read( phoneNumber.NumberTypeValueId ?? 0, rockContext );
                    phoneNumberList += string.Format(
                        "<br>{0} <small>{1}</small>",
                        phoneNumber.IsUnlisted ? "Unlisted" : phoneNumber.NumberFormatted,
                        phoneType != null ? phoneType.Value : string.Empty );
                }

                emailAndPhoneHtml += phoneNumberList + "<p>";

                personInfoHtml.Append( emailAndPhoneHtml );
            }

            personInfoHtml.Append( "</div>" );
            personInfoHtml.Append( "</div>" );

            var dupPersonPnl = new Panel();
            dupPersonPnl.ID = string.Format( "{0}_{1}_{2}", controlIdPrefix, groupMemberGuidString, person.Id );
            dupPersonPnl.Controls.Add( new LiteralControl( personInfoHtml.ToString() ) );

            return dupPersonPnl;
        }

        /// <summary>
        /// Gets the control data.
        /// </summary>
        private void GetControlData()
        {
            GroupMembers = new List<GroupMember>();

            int? childMaritalStatusId = null;
            var childMaritalStatus = DefinedValueCache.Read( GetAttributeValue( "ChildMaritalStatus" ).AsGuid() );
            if ( childMaritalStatus != null )
            {
                childMaritalStatusId = childMaritalStatus.Id;
            }

            int? adultMaritalStatusId = ddlMaritalStatus.SelectedValueAsInt();

            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;

            foreach ( NewGroupMembersRow row in nfmMembers.GroupMemberRows )
            {
                var groupMember = new GroupMember();
                groupMember.GroupMemberStatus = GroupMemberStatus.Active;
                groupMember.Person = new Person();
                groupMember.Person.Guid = row.PersonGuid.Value;
                groupMember.Person.RecordTypeValueId = recordTypePersonId;
                groupMember.Person.RecordStatusValueId = recordStatusActiveId;

                if ( row.RoleId.HasValue )
                {
                    groupMember.GroupRoleId = row.RoleId.Value;

                    if ( _isFamilyGroupType )
                    {
                        if ( groupMember.GroupRoleId == _childRoleId )
                        {
                            groupMember.Person.MaritalStatusValueId = childMaritalStatusId;
                        }
                        else
                        {
                            groupMember.Person.MaritalStatusValueId = adultMaritalStatusId;
                        }
                    }
                    else
                    {
                        groupMember.Person.MaritalStatusValueId = null;
                    }
                }

                groupMember.Person.TitleValueId = row.TitleValueId;
                groupMember.Person.FirstName = row.FirstName;
                groupMember.Person.MiddleName = row.MiddleName;
                groupMember.Person.NickName = groupMember.Person.FirstName;
                groupMember.Person.LastName = row.LastName;
                groupMember.Person.SuffixValueId = row.SuffixValueId;
                groupMember.Person.Gender = row.Gender;

                var birthday = row.BirthDate;
                if ( birthday.HasValue )
                {
                    // If setting a future birthdate, subtract a century until birthdate is not greater than today.
                    var today = RockDateTime.Today;
                    while ( birthday.Value.CompareTo( today ) > 0 )
                    {
                        birthday = birthday.Value.AddYears( -100 );
                    }

                    groupMember.Person.BirthMonth = birthday.Value.Month;
                    groupMember.Person.BirthDay = birthday.Value.Day;

                    if ( birthday.Value.Year != DateTime.MinValue.Year )
                    {
                        groupMember.Person.BirthYear = birthday.Value.Year;
                    }
                    else
                    {
                        groupMember.Person.BirthYear = null;
                    }
                }
                else
                {
                    groupMember.Person.SetBirthDate( null );
                }

                groupMember.Person.ConnectionStatusValueId = row.ConnectionStatusValueId;

                if ( _isFamilyGroupType )
                {
                    groupMember.Person.GradeOffset = row.GradeOffset;
                }

                var contactInfoRow = nfciContactInfo.ContactInfoRows.FirstOrDefault( c => c.PersonGuid == row.PersonGuid );
                if ( contactInfoRow != null )
                {
                    string homeNumber = PhoneNumber.CleanNumber( contactInfoRow.HomePhoneNumber );
                    if ( _homePhone != null && !string.IsNullOrWhiteSpace( homeNumber ) )
                    {
                        var homePhoneNumber = new PhoneNumber();
                        homePhoneNumber.NumberTypeValueId = _homePhone.Id;
                        homePhoneNumber.Number = homeNumber;
                        homePhoneNumber.CountryCode = PhoneNumber.CleanNumber( contactInfoRow.HomePhoneCountryCode );
                        homePhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( homePhoneNumber.CountryCode, homeNumber );
                        groupMember.Person.PhoneNumbers.Add( homePhoneNumber );
                    }

                    string cellNumber = PhoneNumber.CleanNumber( contactInfoRow.CellPhoneNumber );
                    if ( _cellPhone != null && !string.IsNullOrWhiteSpace( cellNumber ) )
                    {
                        var cellPhoneNumber = new PhoneNumber();
                        cellPhoneNumber.NumberTypeValueId = _cellPhone.Id;
                        cellPhoneNumber.Number = cellNumber;
                        cellPhoneNumber.CountryCode = PhoneNumber.CleanNumber( contactInfoRow.CellPhoneCountryCode );
                        cellPhoneNumber.NumberFormatted = PhoneNumber.FormattedNumber( cellPhoneNumber.CountryCode, cellNumber );
                        cellPhoneNumber.IsMessagingEnabled = contactInfoRow.IsMessagingEnabled;
                        groupMember.Person.PhoneNumbers.Add( cellPhoneNumber );
                    }

                    groupMember.Person.Email = contactInfoRow.Email;
                }

                groupMember.Person.IsEmailActive = true;
                groupMember.Person.EmailPreference = EmailPreference.EmailAllowed;

                groupMember.Person.LoadAttributes();

                foreach ( var attributeControl in attributeControls )
                {
                    var attributeRow = attributeControl.AttributesRows.FirstOrDefault( r => r.PersonGuid == row.PersonGuid );
                    if ( attributeRow != null )
                    {
                        attributeRow.GetEditValues( groupMember.Person );
                    }
                }

                GroupMembers.Add( groupMember );
            }
        }

        /// <summary>
        /// Gets the location key.
        /// </summary>
        /// <returns></returns>
        private string GetLocationKey()
        {
            var location = new Location();
            acAddress.GetValues( location );
            return location.GetFullStreetAddress().Trim();
        }

        /// <summary>
        /// Adds the group member.
        /// </summary>
        private void AddGroupMember()
        {
            int defaultRoleId = _groupType.DefaultGroupRoleId ?? _groupType.Roles.Select( r => r.Id ).FirstOrDefault();
            int recordTypePersonId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON.AsGuid() ).Id;
            int recordStatusActiveId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;
            var connectionStatusValue = DefinedValueCache.Read( GetAttributeValue( "DefaultConnectionStatus" ).AsGuid() );

            var person = new Person();
            person.Guid = Guid.NewGuid();
            person.RecordTypeValueId = recordTypePersonId;
            person.RecordStatusValueId = recordStatusActiveId;
            person.Gender = Gender.Unknown;
            person.ConnectionStatusValueId = ( connectionStatusValue != null ) ? connectionStatusValue.Id : (int?)null;

            var groupMember = new GroupMember();
            groupMember.GroupMemberStatus = GroupMemberStatus.Active;
            groupMember.GroupRoleId = defaultRoleId;
            groupMember.Person = person;

            if ( GetAttributeValue( "EnableCommonLastName" ).AsBoolean() )
            {
                if ( GroupMembers.Count > 0 )
                {
                    person.LastName = GroupMembers.FirstOrDefault().Person.LastName;
                }
            }

            GroupMembers.Add( groupMember );
        }

        /// <summary>
        /// Finds the duplicates.
        /// </summary>
        /// <returns></returns>
        public bool FindDuplicates()
        {
            Duplicates = new Dictionary<Guid, List<Person>>();

            var rockContext = new RockContext();
            var locationService = new LocationService( rockContext );
            var groupService = new GroupService( rockContext );
            var personService = new PersonService( rockContext );

            var othersThatMatch = new List<int>();

            foreach ( var person in GroupMembers
                .Where( m =>
                    m.Person != null &&
                    m.Person.FirstName != "" )
                .Select( m => m.Person ) )
            {
                bool otherCriteria = false;
                var personQry = personService
                    .Queryable().AsNoTracking()
                    .Where( p =>
                        p.FirstName == person.FirstName ||
                        p.NickName == person.FirstName );

                if ( othersThatMatch.Any() )
                {
                    personQry = personQry
                        .Where( p => othersThatMatch.Contains( p.Id ) );
                }

                if ( person.BirthDate.HasValue )
                {
                    otherCriteria = true;
                    personQry = personQry
                        .Where( p =>
                            p.BirthDate.HasValue &&
                            p.BirthDate.Value == person.BirthDate.Value );
                }

                if ( _homePhone != null )
                {
                    var homePhoneNumber = person.PhoneNumbers.Where( p => p.NumberTypeValueId == _homePhone.Id ).FirstOrDefault();
                    if ( homePhoneNumber != null )
                    {
                        otherCriteria = true;
                        personQry = personQry
                            .Where( p =>
                                p.PhoneNumbers.Any( n =>
                                    n.NumberTypeValueId == _homePhone.Id &&
                                    n.Number == homePhoneNumber.Number ) );
                    }
                }

                if ( _cellPhone != null )
                {
                    var cellPhoneNumber = person.PhoneNumbers.Where( p => p.NumberTypeValueId == _cellPhone.Id ).FirstOrDefault();
                    if ( cellPhoneNumber != null )
                    {
                        otherCriteria = true;
                        personQry = personQry
                            .Where( p =>
                                p.PhoneNumbers.Any( n =>
                                    n.NumberTypeValueId == _cellPhone.Id &&
                                    n.Number == cellPhoneNumber.Number ) );
                    }
                }

                if ( !string.IsNullOrWhiteSpace( person.Email ) )
                {
                    otherCriteria = true;
                    personQry = personQry
                        .Where( p => p.Email == person.Email );
                }

                var dups = new List<Person>();
                if ( otherCriteria )
                {
                    // If a birthday, email, phone, or address was entered, find anyone with same info and same first name
                    dups = personQry.ToList();
                }
                else
                {
                    // otherwise find people with same first and last name
                    dups = personQry
                        .Where( p => p.LastName == person.LastName )
                        .ToList();
                }

                if ( dups.Any() )
                {
                    Duplicates.Add( person.Guid, dups );
                }
            }

            return Duplicates.Any();
        }

        /// <summary>
        /// Finds the matching addresses
        /// </summary>
        /// <returns></returns>
        public bool FindMatchingAddresses()
        {
            MatchingAddresses = new Dictionary<Guid, List<Person>>();

            var rockContext = new RockContext();
            var locationService = new LocationService( rockContext );
            var groupService = new GroupService( rockContext );
            var personService = new PersonService( rockContext );

            // Find any other group members (any group) that have same location
            IQueryable<Group> othersAtAddress = null;

            string locationKey = GetLocationKey();
            if ( !string.IsNullOrWhiteSpace( locationKey ) && _verifiedLocations.ContainsKey( locationKey ) )
            {
                int? locationId = _verifiedLocations[locationKey];
                if ( locationId.HasValue )
                {
                    var location = locationService.Get( locationId.Value );
                    if ( location != null )
                    {
                        othersAtAddress = groupService
                            .Queryable().AsNoTracking()
                            .Where( g =>
                                g.GroupTypeId == _groupType.Id &&
                                g.GroupLocations.Any( l => l.LocationId == location.Id &&
                                                           l.GroupLocationTypeValueId == _locationType.Id ) )
                            .AsQueryable();
                    }
                }
            }
            
            if ( othersAtAddress != null && othersAtAddress.Any() )
            {
                // since addresses are one per family, only show the head of household
                var dups = othersAtAddress.HeadOfHouseholds().ToList();

                foreach ( var person in GroupMembers
                    .Where( m =>
                        m.Person != null &&
                        m.Person.FirstName != "" )
                    .Select( m => m.Person ) )
                {
                    if ( othersAtAddress.Any() )
                    {
                        MatchingAddresses.Add( person.Guid, dups );
                    }
                }
            }

            return MatchingAddresses.Any();
        }

        /// <summary>
        /// Shows the page.
        /// </summary>
        private void ShowPage()
        {
            pnlGroupData.Visible = CurrentPageIndex == 0;
            pnlContactInfo.Visible = CurrentPageIndex == 1;
            pnlAttributes.Visible = CurrentPageIndex > 1 && CurrentPageIndex <= attributeControls.Count + 1;

            bool showDuplicates = (CurrentPageIndex > attributeControls.Count + 1) && ( phDuplicates.Controls.Count > 0 || phMatchingAddresses.Controls.Count > 0 );

            pnlDuplicateWarning.Visible = showDuplicates;

            attributeControls.ForEach( c => c.Visible = false );
            if ( CurrentPageIndex > 1 && attributeControls.Count >= ( CurrentPageIndex - 1 ) )
            {
                attributeControls[CurrentPageIndex - 2].Visible = true;
            }

            if ( _confirmMaritalStatus && CurrentPageIndex == 0 )
            {
                btnNext.AddCssClass( "js-confirm-marital-status" );
            }
            else
            {
                btnNext.RemoveCssClass( "js-confirm-marital-status" );
            }

            btnPrevious.Visible = CurrentPageIndex > 0;
            btnNext.Text = CurrentPageIndex > attributeControls.Count ?
                (showDuplicates ? "Continue With Add" : "Finish" ) : "Next";

            // If no panels are being show, they have cleared all the duplicates. Provide a message confirming this.
            if ( !pnlGroupData.Visible && !pnlContactInfo.Visible && !pnlAttributes.Visible  && !pnlDuplicateWarning.Visible )
            {
                nbMessages.NotificationBoxType = NotificationBoxType.Success;
                nbMessages.Text = "No more duplicates remain. Select Finish to complete the addition of these individuals.";
            }
        }

        #endregion

    }
}