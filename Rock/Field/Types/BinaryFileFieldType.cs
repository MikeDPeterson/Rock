﻿// <copyright>
// Copyright 2013 by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.UI.Controls;

namespace Rock.Field.Types
{
    /// <summary>
    /// Field Type used to display a dropdown list of binary files of a specific type
    /// </summary>
    [Serializable]
    public class BinaryFileFieldType : FieldType
    {
        /// <summary>
        /// Returns the field's current value(s)
        /// </summary>
        /// <param name="parentControl">The parent control.</param>
        /// <param name="value">Information about the value</param>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="condensed">Flag indicating if the value should be condensed (i.e. for use in a grid column)</param>
        /// <returns></returns>
        public override string FormatValue( Control parentControl, string value, Dictionary<string, ConfigurationValue> configurationValues, bool condensed )
        {
            string formattedValue = string.Empty;

            int id = int.MinValue;
            if (Int32.TryParse( value, out id ))
            {
                var result = new BinaryFileService( new RockContext() )
                    .Queryable()
                    .Select( f => new { f.Id, f.FileName } )
                    .Where( f => f.Id == id )
                    .FirstOrDefault();
                if ( result != null )
                {
                    formattedValue = result.FileName;
                }
            }

            return base.FormatValue( parentControl, formattedValue, null, condensed );
        }

        /// <summary>
        /// Returns a list of the configuration keys
        /// </summary>
        /// <returns></returns>
        public override List<string> ConfigurationKeys()
        {
            var configKeys = base.ConfigurationKeys();
            configKeys.Add( "binaryFileType" );
            return configKeys;
        }

        /// <summary>
        /// Creates the HTML controls required to configure this type of field
        /// </summary>
        /// <returns></returns>
        public override List<Control> ConfigurationControls()
        {
            var controls = base.ConfigurationControls();

            var ddl = new RockDropDownList();
            controls.Add( ddl );
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += OnQualifierUpdated;
            ddl.DataTextField = "Name";
            ddl.DataValueField = "Id";
            ddl.DataSource = new BinaryFileTypeService( new RockContext() )
                .Queryable()
                .OrderBy( f => f.Name )
                .Select( f => new { f.Id, f.Name } )
                .ToList();
            ddl.DataBind();
            ddl.Items.Insert( 0, new ListItem( string.Empty, string.Empty ) );
            ddl.Label = "File Type";
            ddl.Help = "The type of files to list.";

            return controls;
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="controls">The controls.</param>
        /// <returns></returns>
        public override Dictionary<string, ConfigurationValue> ConfigurationValues( List<Control> controls )
        {
            Dictionary<string, ConfigurationValue> configurationValues = new Dictionary<string, ConfigurationValue>();
            configurationValues.Add( "binaryFileType", new ConfigurationValue( "File Type", "The type of files to list", "" ) );

            if ( controls != null && controls.Count == 1 &&
                controls[0] != null && controls[0] is DropDownList )
            {
                configurationValues["binaryFileType"].Value = ( (DropDownList)controls[0] ).SelectedValue;
            }

            return configurationValues;
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="configurationValues"></param>
        public override void SetConfigurationValues( List<Control> controls, Dictionary<string, ConfigurationValue> configurationValues )
        {
            if ( controls != null && controls.Count == 1 && configurationValues != null &&
                controls[0] != null && controls[0] is DropDownList && configurationValues.ContainsKey( "binaryFileType" ) )
            {
                ( (DropDownList)controls[0] ).SelectedValue = configurationValues["binaryFileType"].Value;
            }
        }


        /// <summary>
        /// Creates the control(s) neccessary for prompting user for a new value
        /// </summary>
        /// <param name="configurationValues">The configuration values.</param>
        /// <param name="id"></param>
        /// <returns>
        /// The control
        /// </returns>
        public override Control EditControl( Dictionary<string, ConfigurationValue> configurationValues, string id )
        {
            var control = new BinaryFilePicker { ID = id }; 

            if ( configurationValues != null && configurationValues.ContainsKey( "binaryFileType" ) )
            {
                int definedTypeId = 0;
                if ( Int32.TryParse( configurationValues["binaryFileType"].Value, out definedTypeId ) )
                {
                    control.BinaryFileTypeId = definedTypeId;
                }
            }

            return control;
        }

        /// <summary>
        /// Reads new values entered by the user for the field
        /// </summary>
        /// <param name="control">Parent control that controls were added to in the CreateEditControl() method</param>
        /// <param name="configurationValues"></param>
        /// <returns></returns>
        public override string GetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues )
        {
            if ( control != null && control is BinaryFilePicker )
            {
                return ( (BinaryFilePicker)control ).SelectedValue;
            }
            return null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="configurationValues"></param>
        /// <param name="value">The value.</param>
        public override void SetEditValue( Control control, Dictionary<string, ConfigurationValue> configurationValues, string value )
        {
            int binaryFileId = int.MinValue;
            if ( int.TryParse( value, out binaryFileId ) )
            {
                if ( control != null && control is BinaryFilePicker )
                {
                    ( (BinaryFilePicker)control ).SetValue( binaryFileId.ToString() );
                }
            }
        }

    }
}