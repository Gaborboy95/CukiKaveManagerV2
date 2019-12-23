using CukiKaveManagerV2.JSONObjects.WS;
using CukiKaveManagerV2.UserControls;
using CukiKaveManagerV2.ws.JSONObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CukiKaveManagerV2.ws
{
    class WebSocketHandler
    {
        const string TYPE_NEWMESSAGE = "CREATE";
        const string TYPE_UPDATEMESSAGE = "UPDATE";
        const string TYPE_DELETEMESSAGE = "DELETE";

        GenericWebSocket ws;
        MainWindow main;

        public WebSocketHandler(GenericWebSocket WS, MainWindow main)
        {
            ws = WS;
            this.main = main;
            ws.OnMessageReceived += onMessageReceived;
        }

        private void onMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var baseprd = JsonConvert.DeserializeObject<BaseWSJsonMessage>(e.message);
            MessageBox.Show(baseprd.product.ToString() + " | " + baseprd.type);
            switch(baseprd.type)
            {
                case TYPE_NEWMESSAGE:
                    newMessageReceived(baseprd.product.ToObject<Product>());
                    break;

                case TYPE_UPDATEMESSAGE:
                    updatedMessageReceived(baseprd.product.ToObject<Product>());
                    break;

                case TYPE_DELETEMESSAGE:
                    deletedMessageReceived(baseprd.product.ToObject<DeletedMessage>());
                    break;

                default:
                    Console.WriteLine("UNKNOWN WS TYPE RECEIVED");
                    break;
            }
        }

        void newMessageReceived(Product prd)
        {
            main.Dispatcher.BeginInvoke(new Action(() =>
            {
                var _item = new flippable(prd);
                main.ItemHolder.Children.Add(_item);
                main.Products.Add(prd.id, _item);
            }));
        }

        void updatedMessageReceived(Product prd)
        {
            main.Dispatcher.BeginInvoke(new Action(() =>
            {
                main.Products[prd.id].updateContents(prd);
            }));
        }

        void deletedMessageReceived(DeletedMessage prd)
        {
            main.Dispatcher.BeginInvoke(new Action(() =>
            {
                main.ItemHolder.Children.Remove(main.Products[int.Parse(prd.id)]);
                main.Products.Remove(int.Parse(prd.id));
            }));
        }
    }
}
