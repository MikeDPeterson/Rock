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
using System.Collections.Concurrent;

namespace Rock.Transactions
{
    /// <summary>
    /// 
    /// </summary>
    static public class RockQueue
    {
        /// <summary>
        /// Gets or sets the transaction queue.
        /// </summary>
        /// <value>
        /// The transaction queue.
        /// </value>
        public static ConcurrentQueue<ITransaction> TransactionQueue { get; set; }
        
        /// <summary>
        /// Initializes the <see cref="RockQueue" /> class.
        /// </summary>
        static RockQueue()
        {
            TransactionQueue = new ConcurrentQueue<ITransaction>();
        }
    }
}