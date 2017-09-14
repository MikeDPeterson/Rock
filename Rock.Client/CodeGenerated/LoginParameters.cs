//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rock.CodeGeneration project
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
// <copyright>
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


namespace Rock.Client
{
    /// <summary>
    /// Use this as the Content of a api/Auth/Login POST
    /// </summary>
    public partial class LoginParametersEntity
    {
        /// <summary />
        public string Password { get; set; }

        /// <summary />
        public bool Persisted { get; set; }

        /// <summary />
        public string Username { get; set; }

        /// <summary>
        /// Copies the base properties from a source LoginParameters object
        /// </summary>
        /// <param name="source">The source.</param>
        public void CopyPropertiesFrom( LoginParameters source )
        {
            this.Password = source.Password;
            this.Persisted = source.Persisted;
            this.Username = source.Username;

        }
    }

    /// <summary>
    /// Use this as the Content of a api/Auth/Login POST
    /// </summary>
    public partial class LoginParameters : LoginParametersEntity
    {
    }
}