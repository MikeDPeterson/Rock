using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using church.ccv.Prayer.Model;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;

namespace RockWeb.Plugins.church_ccv.Prayer
{
    [DisplayName( "Prayer Session" )]
    [Category( "CCV > Prayer" )]
    [Description( "Allows a user to start a session to pray for active, approved prayer requests." )]

    [CodeEditorField( "Welcome Introduction Text", "Some text (or HTML) to display on the first step.", CodeEditorMode.Html, height: 100, required: false, defaultValue: "<h2>Let's get ready to pray...</h2>", order: 1 )]
    [CategoryField( "Category", "A top level category. This controls which categories are shown when starting a prayer session.", false, "church.ccv.Prayer.Model.CampusPrayerRequest", "", "", false, "", "Filtering", 2, "CategoryGuid" )]
    [BooleanField( "Enable Prayer Team Flagging", "If enabled, members of the prayer team can flag a prayer request if they feel the request is inappropriate and needs review by an administrator.", false, "Flagging", 3, "EnableCommunityFlagging" )]
    [IntegerField( "Flag Limit", "The number of flags a prayer request has to get from the prayer team before it is automatically unapproved.", false, 1, "Flagging", 4 )]
    public partial class PrayerSession : RockBlock
    {
        #region Fields
        private bool _enableCommunityFlagging = false;
        private string _categoryGuidString = string.Empty;
        private int? _flagLimit = 1;
        private string[] _savedCategoryIdsSetting;
        private string[] _savedCampusIdsSetting;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the note type identifier.
        /// </summary>
        /// <value>
        /// The note type identifier.
        /// </value>
        public int? NoteTypeId
        {
            get { return ViewState["NoteTypeId"] as int?; }
            set { ViewState["NoteTypeId"] = value; }
        }

        /// <summary>
        /// Gets or sets the current prayer request identifier.
        /// </summary>
        /// <value>
        /// The current prayer request identifier.
        /// </value>
        public int? CurrentPrayerRequestId
        {
            get { return ViewState["CurrentPrayerRequestId"] as int?; }
            set { ViewState["CurrentPrayerRequestId"] = value; }
        }

        /// <summary>
        /// Gets or sets the prayer request ids.
        /// </summary>
        /// <value>
        /// The prayer request ids.
        /// </value>
        public List<int> PrayerRequestIds
        {
            get { return ViewState["PrayerRequestIds"] as List<int>; }
            set { ViewState["PrayerRequestIds"] = value; }
        }

        #endregion

        #region Base Control Methods

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            mdFlag.SaveClick += mdFlag_SaveClick;

            _flagLimit = GetAttributeValue( "FlagLimit" ).AsIntegerOrNull();
            _categoryGuidString = GetAttributeValue( "CategoryGuid" );
            _enableCommunityFlagging = GetAttributeValue( "EnableCommunityFlagging" ).AsBoolean();
            lWelcomeInstructions.Text = GetAttributeValue( "WelcomeIntroductionText" );
        }

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                BindCampuses();
                DisplayCategories( cpCampuses );
                SetNoteType();
                lbStart.Focus();
                lbFlag.Visible = _enableCommunityFlagging;
            }

            if ( NoteTypeId.HasValue )
            {
                var noteType = NoteTypeCache.Read( NoteTypeId.Value );
                if ( noteType != null )
                {
                    notesComments.NoteTypes = new List<NoteTypeCache> { noteType };
                }
            }

            notesComments.EntityId = CurrentPrayerRequestId;

            if ( lbNext.Visible )
            {
                lbNext.Focus();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handler that saves the user's category preferences and starts a prayer session
        /// for their selected categories.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbStart_Click( object sender, EventArgs e )
        {
            // Maker sure they selected at least one campus
            if (cpCampuses.SelectedValues.Count == 0 )
            {
                nbSelectCampuses.Visible = true;
                return;
            }
            else
            {
                nbSelectCampuses.Visible = false;
            }

            // Make sure they selected at least one category
            if ( cblCategories.SelectedValues.Count == 0 )
            {
                nbSelectCategories.Visible = true;
                return;
            }
            else
            {
                nbSelectCategories.Visible = false;
            }

            lbNext.Focus();
            lbBack.Visible = false;

            pnlStartSession.Visible = false;

            string settingPrefixCategories = string.Format( "prayer-categories-{0}-", this.BlockId );
            string settingPrefexCampuses = string.Format( "prayer-campus-{0}-", this.BlockId );
            SavePreferences( settingPrefixCategories, settingPrefexCampuses );

            SetAndDisplayPrayerRequests( cblCategories, cpCampuses );


        }

        /// <summary>
        /// Handler that gets the next prayer request and updates its prayer count.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbNext_Click( object sender, EventArgs e )
        {
            int index = hfPrayerIndex.ValueAsInt();

            index++;

            List<int> prayerRequestIds = this.PrayerRequestIds;
            int currentNumber = index + 1;
            if ( ( prayerRequestIds != null ) && ( currentNumber <= prayerRequestIds.Count ) )
            {
                UpdateSessionCountLabel( currentNumber, prayerRequestIds.Count );

                hfPrayerIndex.Value = index.ToString();
                var rockContext = new RockContext();
                Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );
                int prayerRequestId = prayerRequestIds[index];
                CampusPrayerRequest request = prayerRequestService.Queryable( "RequestedByPersonAlias.Person" ).FirstOrDefault( p => p.Id == prayerRequestId );
                ShowPrayerRequest( request, rockContext );
            }
            else
            {
                pnlFinished.Visible = true;
                pnlPrayer.Visible = false;
                lbStartAgain.Focus();
            }

            lbBack.Visible = true;
        }

        /// <summary>
        /// Handles the Click event of the lbPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void lbBack_Click( object sender, EventArgs e )
        {
            int index = hfPrayerIndex.ValueAsInt();

            index--;

            List<int> prayerRequestIds = this.PrayerRequestIds;
            int currentNumber = index + 1;
            if ( ( prayerRequestIds != null ) && ( currentNumber > 0 ) )
            {
                UpdateSessionCountLabel( currentNumber, prayerRequestIds.Count );

                hfPrayerIndex.Value = index.ToString();
                var rockContext = new RockContext();
                Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );
                int prayerRequestId = prayerRequestIds[index];
                CampusPrayerRequest request = prayerRequestService.Queryable( "RequestedByPersonAlias.Person" ).FirstOrDefault( p => p.Id == prayerRequestId );
                ShowPrayerRequest( request, rockContext );

                if ( currentNumber == 1 )
                {
                    lbBack.Visible = false;
                }
            }
        }

        /// <summary>
        /// Handler for when the user has decided to call it quits for their current prayer session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbStop_Click( object sender, EventArgs e )
        {
            pnlFinished.Visible = true;
            pnlPrayer.Visible = false;
        }

        /// <summary>
        /// Handler for when/if the user wants to start a new prayer session.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbStartAgain_Click( object sender, EventArgs e )
        {
            lbBack.Visible = false;
            pnlStartSession.Visible = true;
            pnlFinished.Visible = false;
            pnlNoPrayerRequestsMessage.Visible = false;
            pnlPrayer.Visible = false;
            lbStart.Focus();

            BindCampuses();
            DisplayCategories( cpCampuses );
        }

        /// <summary>
        /// Called when the user clicks on the "Flag" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbFlag_Click( object sender, EventArgs e )
        {
            mdFlag.SaveButtonText = "Yes, Flag This Request";
            mdFlag.Show();
        }

        /// <summary>
        /// Handles the SaveClick event of the mdFlag control and flags the prayer request and moves to the next.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void mdFlag_SaveClick( object sender, EventArgs e )
        {
            int prayerRequestId = hfIdValue.ValueAsInt();

            var rockContext = new RockContext();
            Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );

            CampusPrayerRequest request = prayerRequestService.Get( prayerRequestId );

            if ( request != null )
            {
                request.FlagCount = ( request.FlagCount ?? 0 ) + 1;
                if ( request.FlagCount >= _flagLimit )
                {
                    request.IsApproved = false;
                }

                rockContext.SaveChanges();
            }

            mdFlag.Hide();
            lbNext_Click( sender, e );
        }

        protected void cpCampusesPicker_SelectedIndexChanged( object sender, EventArgs e )
        {
            DisplayCategories( cpCampuses );
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the type of the note.
        /// </summary>
        private void SetNoteType()
        {
            var rockContext = new RockContext();
            NoteTypeService service = new NoteTypeService( rockContext );
            NoteType noteType = service.Get( Rock.SystemGuid.NoteType.PRAYER_COMMENT.AsGuid() );
            NoteTypeId = noteType.Id;
        }

        /// <summary>
        /// Updates the Hightlight label that shows how many prayers have been made out of the total for this session.
        /// </summary>
        /// <param name="currentNumber"></param>
        /// <param name="total"></param>
        private void UpdateSessionCountLabel( int currentNumber, int total )
        {
            hlblNumber.Text = string.Format( "{0} of {1}", currentNumber, total );
        }

        /// <summary>
        /// Binds Campuses to campus picker
        /// </summary>
        private void BindCampuses()
        {
            string settingPrefix = string.Format( "prayer-campus-{0}-", this.BlockId );

            RockContext rockContext = new RockContext();
            Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );

            var campuses = CampusCache.All();

            foreach ( CampusCache campus in campuses)
            {

            }

            cpCampuses.DataSource = CampusCache.All();
            cpCampuses.DataTextField = "Name";
            cpCampuses.DataValueField = "ID";
            cpCampuses.DataBind();

            cpCampuses.SelectedIndexChanged += new EventHandler( cpCampusesPicker_SelectedIndexChanged );
            

            // use the users preferences to set which campus items are checked.
            _savedCampusIdsSetting = this.GetUserPreference( settingPrefix ).SplitDelimitedValues();

            for ( int i = 0; i < cpCampuses.Items.Count; i++ )
            {
                ListItem item = ( ListItem ) cpCampuses.Items[i];
                item.Selected = _savedCampusIdsSetting.Contains( item.Value );

            }
        }

        /// <summary>
        /// Displays any 'active' prayer categories or shows a message if there are none.
        /// </summary>
        private void DisplayCategories( CampusesPicker campuses)
        {
            // If there are no categories, then it means there are no prayer requests (in those categories)
            if ( !BindCategories( _categoryGuidString, campuses ) )
            {
                cblCategories.Visible = false;
                pnlChooseCategories.Visible = false;
                pnlNoPrayerRequestsMessage.Visible = true;
            }
        }

        /// <summary>
        /// Binds the 'active' categories for the given top-level category GUID to the list for 
        /// the user to choose.
        /// </summary>
        /// <param name="categoryGuid">the guid string of a top-level prayer category</param>
        /// <returns>true if there were active categories or false if there were none</returns>
        private bool BindCategories( string categoryGuid, CampusesPicker campuses )
        {
            string settingPrefix = string.Format( "prayer-categories-{0}-", this.BlockId );

            RockContext rockContext = new RockContext();
            Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );

            // Get Active Approved Unexpired prayer requests for the selected campuses
            IQueryable < CampusPrayerRequest > prayerRequestQuery = prayerRequestService.Queryable()
               .Where( p => p.IsActive == true && p.IsApproved == true && RockDateTime.Today <= p.ExpirationDate && campuses.SelectedValues.ToList().Contains( p.CampusId.ToString() ?? "" ) )
               .OrderByDescending( p => p.IsUrgent );


            // Filter categories if one has been selected in the configuration
            if ( !string.IsNullOrEmpty( categoryGuid ) )
            {
                Guid guid = new Guid( categoryGuid );
                var filterCategory = CategoryCache.Read( guid );
                if ( filterCategory != null )
                {
                    prayerRequestQuery = prayerRequestQuery.Where( p => p.Category.ParentCategoryId == filterCategory.Id );
                }
            }

            // get all prayers with a CategoryId
            var prayersWithCounts = prayerRequestQuery
                .Where( p => p.CategoryId != null )
                .Select( p => new { p.CategoryId, p.Category.Name, p.PrayerCount } );

            // get prayers that havent been prayed for, group by category and ID
            var categoriesNeedingPrayer = prayersWithCounts.Where( p => p.PrayerCount == 0 || p.PrayerCount == null ).GroupBy( g => new { g.CategoryId, g.Name } );

            // group all prayers by category, ID, and NeedsPrayer (categoriesNeedingPrayer)
            var prayerCategoriesWithStatus = prayersWithCounts
                .GroupBy( p => new { p.CategoryId, p.Name, NeedsPrayer = categoriesNeedingPrayer.Where( c => c.Key.Name == p.Name ).Count() > 0 ? true : false } );

            // Format list of prayers
            var prayerList = prayerCategoriesWithStatus
                .OrderBy( g => g.Key.Name )
                .Select( a => new
                {
                    Id = a.Key.CategoryId,
                    Name = a.Key.NeedsPrayer == true 
                        ? "<font color=\"red\">" + a.Key.Name + " (" + System.Data.Entity.SqlServer.SqlFunctions.StringConvert( ( double ) a.Count() ).Trim() + ")</Font>"
                        : a.Key.Name + " (" + System.Data.Entity.SqlServer.SqlFunctions.StringConvert( ( double ) a.Count() ).Trim() + ")",
                    NeedsPrayer = a.Key.NeedsPrayer,
                    Count = a.Count()
                } ).ToList();

            // Bind list of prayers to categories check box list
            cblCategories.DataTextField = "Name";
            cblCategories.DataValueField = "Id";
            cblCategories.DataSource = prayerList;
            cblCategories.DataBind();

            // use the users preferences to set which items are checked.
            _savedCategoryIdsSetting = this.GetUserPreference( settingPrefix ).SplitDelimitedValues();
            for ( int i = 0; i < cblCategories.Items.Count; i++ )
            {
                ListItem item = (ListItem)cblCategories.Items[i];
                item.Selected = _savedCategoryIdsSetting.Contains( item.Value );
            }

            return prayerList.Count() > 0;
        }

        /// <summary>
        /// Saves the users selected prayer categories for use during the next prayer session.
        /// </summary>
        /// <param name="settingPrefix"></param>
        private void SavePreferences( string settingPrefixCategories, string settingPrefixCampuses )
        {
            // Save Selected Categories
            var previouslyCheckedCategoryIds = this.GetUserPreference( settingPrefixCategories ).SplitDelimitedValues();

            IEnumerable<string> allCategoryIds = cblCategories.Items.Cast<ListItem>()
                              .Select( i => i.Value );

            // find the items that were previously saved but are no longer in the checkboxlist...
            // because we want to retain those as they may be active categories again in the future.
            var itemsToKeepCategory = previouslyCheckedCategoryIds.Except( allCategoryIds );

            string categoryValues = cblCategories.SelectedValuesAsInt
                .Where( v => v != 0 )
                .Select( c => c.ToString() )
                .Concat( itemsToKeepCategory )
                .ToList()
                .AsDelimited( "," );

            this.SetUserPreference( settingPrefixCategories, categoryValues );

            // Save Selected Campuses
            var previouslyCheckedCampusIds = this.GetUserPreference( settingPrefixCampuses ).SplitDelimitedValues();

            IEnumerable<string> allCampusIds = cpCampuses.Items.Cast<ListItem>()
                .Select( i => i.Value );

            // find the items that were previously saved but are no longer in the checkboxlist...
            // because we want to retain those as they may be active categories again in the future.
            var itemsToKeepCampuses = previouslyCheckedCampusIds.Except( allCampusIds );

            string campusValues = cpCampuses.SelectedValues
                .Concat( itemsToKeepCampuses )
                .ToList()
                .AsDelimited( "," );

            this.SetUserPreference( settingPrefixCampuses, campusValues );
        }

        /// <summary>
        /// Finds all approved prayer requests for the given selected categories and campuses and orders them by least prayed-for.
        /// Also updates the prayer count for the first item in the list.
        /// </summary>
        /// <param name="categoriesList"></param>
        private void SetAndDisplayPrayerRequests( RockCheckBoxList categoriesList, CampusesPicker campuses )
        {
            RockContext rockContext = new RockContext();
            Service<CampusPrayerRequest> service = new Service<CampusPrayerRequest>( rockContext );

            var prayerRequests = GetPrayerRequests( categoriesList.SelectedValuesAsInt, campuses.SelectedValues.AsIntegerList() ).OrderByDescending( p => p.IsUrgent ).ThenBy( p => p.PrayerCount ).ToList();
            List<int> list = prayerRequests.Select( p => p.Id ).ToList<int>();

            PrayerRequestIds = list;
            if ( list.Count > 0 )
            {
                UpdateSessionCountLabel( 1, list.Count );
                hfPrayerIndex.Value = "0";
                CampusPrayerRequest request = prayerRequests.First();
                ShowPrayerRequest( request, rockContext );
            }
            else
            {
                pnlNoPrayerRequestsMessage.Visible = true;
            }
        }

        /// <summary>
        /// Displays the details for a single, given prayer request.
        /// </summary>
        /// <param name="prayerRequest">The prayer request.</param>
        /// <param name="rockContext">The rock context.</param>
        private void ShowPrayerRequest( CampusPrayerRequest prayerRequest, RockContext rockContext )
        {
            pnlPrayer.Visible = true;
            divPrayerAnswer.Visible = false;

            prayerRequest.PrayerCount = ( prayerRequest.PrayerCount ?? 0 ) + 1;
            hlblPrayerCountTotal.Text = prayerRequest.PrayerCount.ToString() + " team prayers";
            hlblUrgent.Visible = prayerRequest.IsUrgent ?? false;
            hlblCampus.Text = CampusCache.Read( prayerRequest.CampusId ?? 1 ).Name;
            lTitle.Text = prayerRequest.FullName.FormatAsHtmlTitle();

            lPrayerText.Text = prayerRequest.Text.ScrubHtmlAndConvertCrLfToBr();

            if ( prayerRequest.EnteredDateTime != null )
            {
                lPrayerRequestDate.Text = string.Format( "Date Entered: {0}", prayerRequest.EnteredDateTime.ToShortDateString() );
            }

            hlblCategory.Text = prayerRequest.Category.Name;

            // Show their answer if there is one on the request.
            if ( !string.IsNullOrWhiteSpace( prayerRequest.Answer ) )
            {
                divPrayerAnswer.Visible = true;
                lPrayerAnswerText.Text = prayerRequest.Answer.EncodeHtml().ConvertCrLfToHtmlBr();
            }

            // put the request's id in the hidden field in case it needs to be flagged.
            hfIdValue.SetValue( prayerRequest.Id );

            if ( prayerRequest.RequestedByPersonAlias != null )
            {
                lPersonIconHtml.Text = Person.GetPersonPhotoImageTag( prayerRequest.RequestedByPersonAlias, 50, 50, "pull-left margin-r-md img-thumbnail" );
            }
            else
            {
                lPersonIconHtml.Text = string.Empty;
            }

            pnlPrayerComments.Visible = prayerRequest.AllowComments ?? false;
            if ( notesComments.Visible )
            {
                notesComments.EntityId = prayerRequest.Id;
                notesComments.RebuildNotes( true );
            }

            CurrentPrayerRequestId = prayerRequest.Id;

            try
            {
                // save because the prayer count was just modified.
                rockContext.SaveChanges();
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, Context, this.RockPage.PageId, this.RockPage.Site.Id, CurrentPersonAlias );
            }
        }

        /// <summary>
        /// Returns a collection of active <see cref="church.ccv.Prayer.Model.CampusPrayerRequest">PrayerRequests</see> that
        /// are in a specified <see cref="Rock.Model.Category"/> or any of its subcategories for specified <see cref="Rock.Model.Campus"/>
        /// </summary>
        /// <param name="categoryIds">A <see cref="System.Collections.Generic.List{Int32}"/> of
        /// the <see cref="Rock.Model.Category"/> IDs to retrieve PrayerRequests for.</param>
        /// <param name="campusIds">A <see cref="System.Collections.Generic.List{Int32}"/> of
        /// the <see cref="Rock.Model.Campus"/> IDs to retrieve PrayerRequests for. </param>
        /// <param name="onlyApproved">set false to include un-approved requests.</param>
        /// <param name="onlyUnexpired">set false to include expired requests.</param>
        /// <returns>An enumerable collection of <see cref="church.ccv.Prayer.Model.CampusPrayerRequest"/> that
        /// are in the specified <see cref="Rock.Model.Category"/> or any of its subcategories.</returns>
        private IEnumerable<CampusPrayerRequest> GetPrayerRequests( List<int> categoryIds, List<int> campusIds, bool onlyApproved = true, bool onlyUnexpired = true )
        {
            RockContext rockContext = new RockContext();

            CampusPrayerRequest prayerRequest = new CampusPrayerRequest();
            Type type = prayerRequest.GetType();
            var prayerRequestEntityTypeId = EntityTypeCache.GetId( type );

            // Get all PrayerRequest category Ids that are the **parent or child** of the given categoryIds.
            CategoryService categoryService = new CategoryService( rockContext );
            IEnumerable<int> expandedCategoryIds = categoryService.GetByEntityTypeId( prayerRequestEntityTypeId )
                .Where( c => categoryIds.Contains( c.Id ) || categoryIds.Contains( c.ParentCategoryId ?? -1 ) )
                .Select( a => a.Id );

            // Now find the active PrayerRequests that have any of those category Ids for the campuses selected.
            Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );
            IQueryable<CampusPrayerRequest> list = prayerRequestService.Queryable()
                .Where( p => p.IsActive == true && expandedCategoryIds.Contains( p.CategoryId ?? -1 ) && campusIds.Contains( p.CampusId ?? -1 ) );

            if ( onlyApproved )
            {
                list = list.Where( p => p.IsApproved == true );
            }

            if ( onlyUnexpired )
            {
                list = list.Where( p => RockDateTime.Today <= p.ExpirationDate );
            }

            return list;
        }

        #endregion
    }
}