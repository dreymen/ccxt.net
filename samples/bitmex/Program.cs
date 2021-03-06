﻿using System;
using System.Threading.Tasks;
using CCXT.NET.Shared.Coin.Types;

namespace bitmex
{
    class Program
    {
        private static int __step_no = 1;

        private static readonly string API_KEY = "";    // Put your API key
        private static readonly string API_SECRET = ""; // Put your API secret;
        private static decimal initial_sell_limit = 20000m;
        private static decimal amended_sell_limit = 19500m;
        private static decimal order_quantity = 100m;

        private static async Task Main(string[] args)
        {
            var _public_api = new CCXT.NET.BitMEX.Public.PublicApi();
            var _private_api = new CCXT.NET.BitMEX.Private.PrivateApi(API_KEY, API_SECRET, is_live: false);
            var _trade_api = new CCXT.NET.BitMEX.Trade.TradeApi(API_KEY, API_SECRET, is_live: false);

            if (__step_no == 0)
            {
                var _ohlcvs = await _public_api.FetchOHLCVsAsync("btc", "usd");
                Console.WriteLine(_ohlcvs.result.Count);
            }

            if (__step_no == 1)
            {
                Console.Out.WriteLine($"Placing sell limit order at {initial_sell_limit}...");
                var _limit_order = await _trade_api.CreateLimitOrderAsync("BTC", "USD", order_quantity, initial_sell_limit, SideType.Ask);

                if (_limit_order.result.orderStatus == OrderStatus.Open)
                {
                    Console.Out.WriteLine($"Changing limit of the sell order to {amended_sell_limit}...");
                    _limit_order = await _trade_api.UpdateOrder("BTC", "USD", _limit_order.result.orderId, _limit_order.result.quantity, amended_sell_limit, _limit_order.result.sideType);
                }

                if (_limit_order.result.orderStatus == OrderStatus.Open)
                {
                    Console.Out.WriteLine("Cancelling order...");
                    _limit_order  = await _trade_api.CancelOrderAsync("BTC", "USD", _limit_order.result.orderId, 0m, 0m, SideType.Unknown);
                }
                if (_limit_order.result.orderStatus == OrderStatus.Canceled)
                {
                    Console.Out.WriteLine("Order successfully cancelled.");
                }
            }

            Console.Out.WriteLine("hit return to exit...");
            Console.ReadLine();
        }
    }
}
