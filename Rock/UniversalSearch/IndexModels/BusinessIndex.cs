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
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.UniversalSearch.IndexModels
{
    /// <summary>
    /// Business Index
    /// </summary>
    /// <seealso cref="Rock.UniversalSearch.IndexModels.IndexModelBase" />
    public class BusinessIndex : IndexModelBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>
        /// The contacts.
        /// </value>
        public string Contacts { get; set; }

        /// <summary>
        /// Gets the icon CSS class.
        /// </summary>
        /// <value>
        /// The icon CSS class.
        /// </value>
        public override string IconCssClass
        {
            get
            {
                return "fa fa-building";
            }
        }

        /// <summary>
        /// Loads by model.
        /// </summary>
        /// <param name="business">The business.</param>
        /// <returns></returns>
        public static BusinessIndex LoadByModel(Person business )
        {
            var businessIndex = new BusinessIndex();
            businessIndex.SourceIndexModel = "Rock.Model.Person";
            businessIndex.ModelConfiguration = "nofilters";

            businessIndex.ModelOrder = 6;

            businessIndex.Id = business.Id;
            businessIndex.Name = business.LastName;
            
            // do not currently index business attributes since they are shared with people
            //AddIndexableAttributes( businessIndex, person );

            var knownRelationshipGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS.AsGuid() );
            var knownRelationshipOwnerRoleId = knownRelationshipGroupType.Roles.Where( r => r.Guid == SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER.AsGuid() ).FirstOrDefault().Id;
            var knownRelationshipBusinessContactId = knownRelationshipGroupType.Roles.Where( r => r.Guid == SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_BUSINESS_CONTACT.AsGuid() ).FirstOrDefault().Id;

            RockContext rockContext = new RockContext();
            var contactGroup = new GroupMemberService( rockContext ).Queryable()
                                        .Where( m =>
                                             m.Group.GroupTypeId == knownRelationshipGroupType.Id
                                             && m.GroupRoleId == knownRelationshipOwnerRoleId
                                             && m.PersonId == business.Id)
                                        .FirstOrDefault();

            if ( contactGroup != null )
            {
                var contacts = new GroupMemberService( rockContext ).Queryable().AsNoTracking()
                                    .Where( m =>
                                         m.Group.GroupTypeId == knownRelationshipGroupType.Id
                                         && m.GroupId == contactGroup.GroupId
                                         && m.GroupRoleId == knownRelationshipBusinessContactId )
                                    .Select( m => m.Person.NickName + " " + m.Person.LastName ).ToList();

                if ( contacts != null )
                {
                    businessIndex.Contacts = string.Join( " ", contacts );
                }
            }

            return businessIndex;
        }

        /// <summary>
        /// Gets the document URL.
        /// </summary>
        /// <param name="displayOptions"></param>
        /// <returns></returns>
        public override string GetDocumentUrl( Dictionary<string, object> displayOptions = null )
        {
            string url = "/Business/";

            if ( displayOptions != null )
            {
                if ( displayOptions.ContainsKey( "Business.Url" ) )
                {
                    url = displayOptions["Business.Url"].ToString();
                }
            }

            return url + this.Id;
        }

        /// <summary>
        /// Formats the search result.
        /// </summary>
        /// <param name="person"></param>
        /// <param name="displayOptions"></param>
        /// <returns></returns>
        public override FormattedSearchResult FormatSearchResult( Person person, Dictionary<string, object> displayOptions = null )
        {
            string url = "/Business/";

            if (displayOptions != null )
            {
                if ( displayOptions.ContainsKey( "Business.Url" ) )
                {
                    url = displayOptions["Business.Url"].ToString();
                }
            }

            return new FormattedSearchResult() { IsViewAllowed = true, FormattedResult = $@"
                         <div class='row model-cannavigate' data-href='{url}{this.Id}'>
                            <div class='col-sm-1 text-center'>
                                <i class='{this.IconCssClass} fa-2x'></i>
                            </div>
                            <div class='col-sm-11'>
                                {this.Name}
                            </div>
                        </div>
                    " };
        }
    }
}
