using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.Templates
{
    public class EmailTemplates
    {
        public static string EmailLinkTemplate(string callbackUrl)
        {
            var msgStart = @"
                <html>
                    <head>
                        <meta charset='utf-8' />
                        <title></title>
                        <style>
                            @mixin media()
                            {
                                @media(min-width: 768px)
                                {
                                    @content   
                                }

                            }
                            body, html
                            {
                                font-family: 'Vollkorn', serif;
	                            font-weight: 400;
	                            line-height: 1.3;
	                            font-size: 16px;
                            }
                            
                            header, main, footer
                            {
                                max-width: 960px;
                                margin: 0 auto;
                            }
                            .title
                            {
                                font-size: 24px;
	                            color: black;  
	                            text-align: center;
	                            font-weight: 700;
	                            color: #181818;
	                            text-shadow: 0px 2px 2px #a6f8d5;
	                            position: relative;
	                            margin: 0 0 20px 0;
	                            
	                            @include media 
                                {
	                            	font-size: 30px;
	                            }
                            }
                            table
                            {
                                width:100%;
                                border: 1px solid;
                                @include media
                                {
                                    font-size:10px;
                                }
                            }
                            th
                            {
                                background-color: gray;
                            }
                            td, th
                            {
                                text-align: center;
                                vertical-align: middle;
                            }
                            .margin-top
                            {
                                margin-top: 25px;
                            }
                        </style>
                    </head>
                    <body>";

            var emailBody = $"Please confirm your email address " +
               $"<a href=\"#URL#\">Click here</a>";
            var body = emailBody.Replace("#URL#", callbackUrl);
            string msgEnd = "</body></html>";

            return msgStart + body + msgEnd;

        }
        public static string ExchangePendingTemplate(ItemExchangeRequest itemExchangeRequest)
        {
            bool haveGoodExchange = itemExchangeRequest.exchangeOrderItems.Count > 0;
            bool haveBadExchange = itemExchangeRequest.exchangeItemsCanceled.Count > 0;
            bool havePendingExchange = itemExchangeRequest.exchangeItemsPending.Count > 0;


            string msgStart = @"
                <html>
                    <head>
                        <meta charset='utf-8' />
                        <title></title>
                        <style>
                            @mixin media()
                            {
                                @media(min-width: 768px)
                                {
                                    @content   
                                }

                            }
                            body, html
                            {
                                font-family: 'Vollkorn', serif;
	                            font-weight: 400;
	                            line-height: 1.3;
	                            font-size: 16px;
                            }
                            
                            header, main, footer
                            {
                                max-width: 960px;
                                margin: 0 auto;
                            }
                            .title
                            {
                                font-size: 24px;
	                            color: black;  
	                            text-align: center;
	                            font-weight: 700;
	                            color: #181818;
	                            text-shadow: 0px 2px 2px #a6f8d5;
	                            position: relative;
	                            margin: 0 0 20px 0;
	                            
	                            @include media 
                                {
	                            	font-size: 30px;
	                            }
                            }
                            table
                            {
                                width:100%;
                                border: 1px solid;
                                @include media
                                {
                                    font-size:10px;
                                }
                            }
                            th
                            {
                                background-color: gray;
                            }
                            td, th
                            {
                                text-align: center;
                                vertical-align: middle;
                            }
                            .margin-top
                            {
                                margin-top: 25px;
                            }
                        </style>
                    </head>
                    <body>
                        <p>Good day,</p>
                        <p>Items witch will be refunded</p>";

            string tableRefund = "";

            if (haveGoodExchange)
            {
                string tableStart = "<table> " +
                    "<tr>" +
                    "<th>Returned item name</th>" +
                    "<th>Returned Item Size</th>" +
                    "<th>Returned Item Color</th>" +
                    "<th>Exchanged Item Size</th>" +
                    "<th>Exchange Item Color</th>" +
                    "<th>Quantity</th>" +
                    "</tr>";
                string tableItem = "";
                foreach (var item in itemExchangeRequest.exchangeOrderItems)
                {
                    string tableItemWithInfo = "<tr>" +
                        "<td>{{ReturnedItemName}}</td>".Replace("{{ReturnedItemName}}", item.BaseProductName) +
                        "<td>{{ReturnedItemSize}}</td>".Replace("{{ReturnedItemSize}}", item.ReturnedProductVariantSize) +
                        "<td>{{ReturnedItemColor}}</td>".Replace("{{ReturnedItemColor}}", item.ReturnedProductVariantColor) +
                        "<td>{{ExchangedItemSize}}</td>".Replace("{{ExchangedItemSize}}", item.ExchangedProductVariantSize) +
                        "<td>{{ExchangedItemColor}}</td>".Replace("{{ExchangedItemColor}}", item.ExchangedProductVariantColor) +
                        "<td>{{Quantity}}</td>".Replace("{{Quantity}}", item.Quantity.ToString()) +
                        "</tr>";
                    tableItem += tableItemWithInfo;
                }
                string tableEnd = "</table>";

                tableRefund = tableStart + tableItem + tableEnd;
            }

            string tableNotRefund = "";
            if (haveBadExchange == true)
            {
                tableNotRefund = "<p class='margin-top'> Items witch we cannot refund </p>";
            }
            else
            {
                tableNotRefund = "<p class='margin-top'> All items refunded </p>";
            }

            if (haveBadExchange)
            {
                string tableNotRefunded = "<p class='margin-top'> Items witch we cannot refund </p>";

                string tableStart = "<table class='margin-top'> " +
                    "<tr>" +
                    "<th>Item Name</th>" +
                    "<th>Item Color</th>" +
                    "<th>Item Size</th>" +
                    "<th>Quantity</th>" +
                     "<th>Price per unit</th>" +
                     "<th>Reason for no refund</th>" +
                    "</tr>";
                string tableItem = "";
                foreach (var item in itemExchangeRequest.exchangeItemsCanceled)
                {
                    string tableItemWithInfo = "<tr>" +
                        "<td>{{ItemName}}</td>".Replace("{{ItemName}}", item.BaseProductName) +
                        "<td>{{ItemColor}}</td>".Replace("{{ItemColor}}", item.ReturnedProductVariantColor) +
                        "<td>{{ItemSize}}</td>".Replace("{{ItemSize}}", item.ReturnedProductVariantSize) +
                        "<td>{{Quantity}}</td>".Replace("{{Quantity}}", item.Quantity.ToString()) +
                         "<td>{{PricePerUnit}}</td>".Replace("{{PricePerUnit}}", item.Quantity.ToString()) +
                         "<td>{{NotRefundReasons}}</td>".Replace("{{NotRefundReasons}}", item.CancelationReason) +
                        "</tr>";
                    tableItem += tableItemWithInfo;
                }
                string tableEnd = "</table>";

                tableNotRefund = tableNotRefunded + tableStart + tableItem + tableEnd;
            }
            string tablePending = "";
            if (havePendingExchange)
            {
                string tablePendingMsg = "<p class='margin-top'> Items witch are pending </p>";

                string tableStart = "<table class='margin-top'> " +
                    "<tr>" +
                    "<th>Item Name</th>" +
                    "<th>Item Color</th>" +
                    "<th>Item Size</th>" +
                    "<th>Quantity</th>" +
                     "<th>Price per unit</th>" +
                     "<th>Reason why pending</th>" +
                    "</tr>";
                string tableItem = "";
                foreach (var item in itemExchangeRequest.exchangeItemsPending)
                {
                    string tableItemWithInfo = "<tr>" +
                        "<td>{{ItemName}}</td>".Replace("{{ItemName}}", item.BaseProductName) +
                        "<td>{{ItemColor}}</td>".Replace("{{ItemColor}}", item.ReturnedProductVariantColor) +
                        "<td>{{ItemSize}}</td>".Replace("{{ItemSize}}", item.ReturnedProductVariantSize) +
                        "<td>{{Quantity}}</td>".Replace("{{Quantity}}", item.Quantity.ToString()) +
                         "<td>{{PricePerUnit}}</td>".Replace("{{PricePerUnit}}", item.Quantity.ToString()) +
                         "<td>{{NotRefundReasons}}</td>".Replace("{{NotRefundReasons}}", item.Message) +
                        "</tr>";
                    tableItem += tableItemWithInfo;
                }
                string tableEnd = "</table>";

                tablePending = tablePendingMsg + tableStart + tableItem + tableEnd;
            }


            string rules = "<h2>Return policy</h2>" +
                            "<ol>" +
                            "<li>Items can be returned 14 days after recieved in item</li>" +
                            "<li>Items damaged by customer wont be refunded</li>" +
                            "<li>Items with visible wear and tear wont be refunded</li>" +
                            "</ol>";

            string msgEnd = "</body></html>";

            string htmlMsg = msgStart + tableRefund + tableNotRefund + tablePending + rules + msgEnd;

            return htmlMsg;
        }

        public static string RefundTemplate(ItemReturnRequest itemReturnRequest)
        {
            string firstTableString = "";
            bool haveRefundTable = itemReturnRequest.itemsGoodForRefund.Count > 0;
            bool haveBadRefundTable = itemReturnRequest.itemsBadForRefund.Count > 0;

            var msgStart = @"
                <html>
                    <head>
                        <meta charset='utf-8' />
                        <title></title>
                        <style>
                            @mixin media()
                            {
                                @media(min-width: 768px)
                                {
                                    @content   
                                }

                            }
                            body, html
                            {
                                font-family: 'Vollkorn', serif;
	                            font-weight: 400;
	                            line-height: 1.3;
	                            font-size: 16px;
                            }
                            
                            header, main, footer
                            {
                                max-width: 960px;
                                margin: 0 auto;
                            }
                            .title
                            {
                                font-size: 24px;
	                            color: black;  
	                            text-align: center;
	                            font-weight: 700;
	                            color: #181818;
	                            text-shadow: 0px 2px 2px #a6f8d5;
	                            position: relative;
	                            margin: 0 0 20px 0;
	                            
	                            @include media 
                                {
	                            	font-size: 30px;
	                            }
                            }
                            table
                            {
                                width:100%;
                                border: 1px solid;
                                @include media
                                {
                                    font-size:10px;
                                }
                            }
                            th
                            {
                                background-color: gray;
                            }
                            td, th
                            {
                                text-align: center;
                                vertical-align: middle;
                            }
                            .margin-top
                            {
                                margin-top: 25px;
                            }
                        </style>
                    </head>
                    <body>
                        <p>Good day,</p>
                        <p>Items witch will be refunded</p>";

            string tableRefund = "";

            if (haveRefundTable)
            {
                string tableStart = "<table> " +
                    "<tr>" +
                    "<th>Item Name</th>" +
                    "<th>Item Color</th>" +
                    "<th>Item Size</th>" +
                    "<th>Quantity</th>" +
                     "<th>Price per unit</th>" +
                    "</tr>";
                string tableItem = "";
                foreach (var item in itemReturnRequest.itemsGoodForRefund)
                {
                    string tableItemWithInfo = "<tr>" +
                        "<td>{{ItemName}}</td>".Replace("{{ItemName}}", item.BaseProductName) +
                        "<td>{{ItemColor}}</td>".Replace("{{ItemColor}}", item.ProductColor) +
                        "<td>{{ItemSize}}</td>".Replace("{{ItemSize}}", item.ProductSize) +
                        "<td>{{Quantity}}</td>".Replace("{{Quantity}}", item.Quantity.ToString()) +
                         "<td>{{PricePerUnit}}</td>".Replace("{{PricePerUnit}}", item.PricePaidPerItem.ToString()) +
                        "</tr>";
                    tableItem += tableItemWithInfo;
                }
                string tableEnd = "</table>";

                tableRefund = tableStart + tableItem + tableEnd;
            }

            string tableNotRefund = "";
            if (haveBadRefundTable == true)
            {
                tableNotRefund = "<p class='margin-top'> Items witch we cannont refund </p>";
            }
            else
            {
                tableNotRefund = "<p class='margin-top'> All items refunded </p>";
            }

            if (haveBadRefundTable)
            {
                string tableNotRefunded = "<p class='margin-top'> Items witch we cannot refund </p>";

                string tableStart = "<table class='margin-top'> " +
                    "<tr>" +
                    "<th>Item Name</th>" +
                    "<th>Item Color</th>" +
                    "<th>Item Size</th>" +
                    "<th>Quantity</th>" +
                     "<th>Price per unit</th>" +
                     "<th>Reason for no refund</th>" +
                    "</tr>";
                string tableItem = "";
                foreach (var item in itemReturnRequest.itemsBadForRefund)
                {
                    string tableItemWithInfo = "<tr>" +
                        "<td>{{ItemName}}</td>".Replace("{{ItemName}}", item.BaseProductName) +
                        "<td>{{ItemColor}}</td>".Replace("{{ItemColor}}", item.ProductColor) +
                        "<td>{{ItemSize}}</td>".Replace("{{ItemSize}}", item.ProductSize) +
                        "<td>{{Quantity}}</td>".Replace("{{Quantity}}", item.Quantity.ToString()) +
                         "<td>{{PricePerUnit}}</td>".Replace("{{PricePerUnit}}", item.PricePaidPerItem.ToString()) +
                         "<td>{{NotRefundReasons}}</td>".Replace("{{NotRefundReasons}}", item.ReasonForNotRefunding) +
                        "</tr>";
                    tableItem += tableItemWithInfo;
                }
                string tableEnd = "</table>";

                tableNotRefund = tableNotRefunded + tableStart + tableItem + tableEnd;
            }

            string totalAmountRefunded = "<p class='margin-top'>We will refund your items in 14 days, total refund amount: "
                + itemReturnRequest.totalAmountRefunded.ToString() + "</p>";

            string AmountTable =
                "<table class='margin-top'>" +
                    "<tr>" +
                    "<th>Total Requested Refund</th>" +
                    "<th>Not Refunded Amount</th>" +
                    "<th>Refunded Amount</th>" +
                    "</tr>" +
                    "<tr>" +
                    "<td>{{RefundRequest}}</td>".Replace("{{RefundRequest}}", itemReturnRequest.totalRequestForRefund.ToString()) +
                    "<td>{{AmountNotRefunded}}</td>".Replace("{{AmountNotRefunded}}", itemReturnRequest.totalAmountNotRefunded.ToString()) +
                    "<td>{{AmountRefunded}}</td>".Replace("{{AmountRefunded}}", itemReturnRequest.totalAmountRefunded.ToString()) +
                    "</tr>" +
                "</table>";


            string rules = "<h2>Return policy</h2>" +
                            "<ol>" +
                            "<li>Items can be returned 14 days after receiving item</li>" +
                            "<li>Items damaged by customer wont be refunded</li>" +
                            "<li>Items with visible wear and tear wont be refunded</li>" +
                            "</ol>";

            string msgEnd = "</body></html>";

            string htmlMsg = msgStart + tableRefund + tableNotRefund + AmountTable + rules + msgEnd;

            return htmlMsg;
        }

    }
}
