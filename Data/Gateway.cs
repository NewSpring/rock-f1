// <copyright>
// Copyright by the Spark Development Network
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
using Rock.Attribute;
using Rock.Financial;
using Rock.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using RestSharp;

namespace cc.newspring.F1
{
    /// <summary>
    /// NMI Payment Gateway
    /// </summary>
    [Description( "F1 Gateway" )]
    [Export( typeof( GatewayComponent ) )]
    [ExportMetadata( "ComponentName", "F1 Gateway" )]

    [TextField( "Vendor Key", "The API vendor key" )]
    [TextField("Base Url", "The base url to use to check the credentials against", true, "https://newspring.fellowshiponeapi.com/")]
    public class Gateway : GatewayComponent
    {
        public override FinancialScheduledTransaction AddScheduledPayment(FinancialGateway financialGateway, PaymentSchedule schedule, PaymentInfo paymentInfo, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool CancelScheduledPayment(FinancialScheduledTransaction transaction, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override FinancialTransaction Charge(FinancialGateway financialGateway, PaymentInfo paymentInfo, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override FinancialTransaction Credit(FinancialTransaction origTransaction, decimal amount, string comment, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the payments that have been processed for any scheduled transactions
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public override List<Payment> GetPayments(FinancialGateway financialGateway, DateTime startDate, DateTime endDate, out string errorMessage)
        {
            errorMessage = null;

            var baseUrl = GetAttributeValue("BaseUrl");
            var key = GetAttributeValue("VendorKey");
            var auth = string.Format("BASIC {0}", Base64Encode(key));
            var recordsPerPage = 100;
            endDate = endDate.AddDays(1);

            var resourceUrl = string.Format("giving/v1/contributionreceipts/search?recordsPerPage={0}&startReceivedDate={1}&endReceivedDate={2}", 
                recordsPerPage, GetQueryParam(startDate), GetQueryParam(endDate));

            var restClient = new RestClient(baseUrl);
            var restRequest = new RestRequest(resourceUrl, Method.GET);
            restRequest.AddHeader("Authorization", auth);
            restRequest.AddHeader("Content-Type", "application/json");

            var restResponse = restClient.Execute(restRequest);
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
            object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
            Response jsonResponse
            = objResponse as Response;
            return jsonResponse;

            return new List<Payment>();
        }

        public override string GetReferenceNumber(FinancialScheduledTransaction scheduledTransaction, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override string GetReferenceNumber(FinancialTransaction transaction, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool GetScheduledPaymentStatus(FinancialScheduledTransaction transaction, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool ReactivateScheduledPayment(FinancialScheduledTransaction transaction, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateScheduledPayment(FinancialScheduledTransaction transaction, PaymentInfo paymentInfo, out string errorMessage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Base64s the plainText.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Returns a query param value for the given dateTime.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private static string GetQueryParam(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}
