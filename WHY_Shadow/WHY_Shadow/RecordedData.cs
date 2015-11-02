using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

using CIPC_CS;

namespace WHY_Shadow
{
    class RecordedData : ICamera
    {

        CIPCClient.ReceiveClient newClient;

        Mat receivedimage;
        ShadowPackage package;
        //Window win;

        public RecordedData()
        {
            this.Add_Client_Click();
            this.package = new ShadowPackage();
            //6this.receivedimage = new Mat();

            //this.win = new Window("img");
        }

        private void Add_Client_Click(/*object sender, System.Windows.RoutedEventArgs e*/)
        {

            int myPort = 56000;
            string serverIP = "127.0.0.1";
            int serverPort = 50000;
            string clientName = "Reciever";
            int fps = 30;
            int id = 0;

            try
            {
                if (this.newClient != null) return;
                //新しくアカウントを立てる
                newClient = this.Create_CIPCClient(myPort, serverIP, serverPort, clientName, fps, id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        CIPCClient.ReceiveClient Create_CIPCClient(int myPort, string serverIP, int serverPort, string clientName, int fps, int id)
        {
            this.newClient = new CIPCClient.ReceiveClient(myPort, serverIP, serverPort, clientName, fps, id);
            try
            {
                newClient.Setup(CIPC_CS.CLIENT.MODE.Both);
                newClient.DataReceived += this.DataReceived;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return newClient;
        }
        public void DataReceived(object sender, byte[] e)
        {

            try
            {
                CIPCClient.ReceiveClient senderclient = (CIPCClient.ReceiveClient)sender;

                this.receivedimage = Mat.FromImageData(e, LoadMode.GrayScale);

                //this.win.ShowImage(this.receivedimage);

            
                //this._showImageEvent();
                //mat.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }


        public ShadowPackage GetImage()
        {
            this.package.srcMat = this.receivedimage.Clone();
            return this.package;
        }

        public void OnClose()
        {
            this.newClient.Close();
        }
    }
}
