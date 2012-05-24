using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace Tools.SendUdp
{
    public delegate void DelegateShow(string str);
    public partial class Form1 : Form
    {
        public Socket udpOut;
        protected byte[] m_udpBuf;
        protected AsyncCallback m_udpReceiveCallback;
        public DelegateShow RecShow;

        /// <summary>
        /// Gets the UDP buffer of this server instance
        /// </summary>
        protected byte[] UDPBuffer
        {
            get
            {
                return m_udpBuf;
            }
        }
               
        public Form1()
        {
            InitializeComponent();
            udpOut = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpOut.Bind(new IPEndPoint(IPAddress.Any, 5043));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbxContent.Text))
            {
                string[] temp = tbxContent.Text.Split(' ');
                List<byte> list = new List<byte>();
                foreach (string s in temp)
                {
                    if (s.Length == 2)
                    {
                        string t = "0x" + s;
                        list.Add(Convert.ToByte(t,16));
                    }
                }

                IPEndPoint toAddr = new IPEndPoint(IPAddress.Parse(tbxServer.Text), int.Parse(tbxPort.Text));
                
                byte[] buffer = list.ToArray();
                udpOut.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, toAddr, new AsyncCallback(SendToCallback), udpOut);
            }
        }

        /// <summary>
        /// Callback function for UDP sends
        /// </summary>
        /// <param name="ar">Asynchronous result of this operation</param>
        protected void SendToCallback(IAsyncResult ar)
        {
            if (ar == null) return;
            try
            {
                Socket s = (Socket)(ar.AsyncState);
                s.EndSendTo(ar);
                tbxContent.Text = null;
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
            catch (Exception)
            {
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RecShow = new DelegateShow(ShowRec);
            m_udpReceiveCallback = new AsyncCallback(RecvFromCallback);
            m_udpBuf = new byte[2048];
            udpOut = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpOut.Bind(new IPEndPoint(IPAddress.Any, int.Parse(tbxPort.Text)));
            BeginReceiveUDP(udpOut, this);
        }

        public void ShowRec(string str)
        {
            txtRec.Text = str;
        }


        /// <summary>
        /// UDP event handler. Called when a UDP packet is waiting to be read
        /// </summary>
        /// <param name="ar"></param>
        protected void RecvFromCallback(IAsyncResult ar)
        {

            Form1 server = (Form1)(ar.AsyncState);
            Socket s = server.udpOut;
            byte[] data = null;
            if (s != null)
            {
                //Creates a temporary EndPoint to pass to EndReceiveFrom.
                EndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
                try
                {
                    // Handle the packet
                    int read = s.EndReceiveFrom(ar, ref tempRemoteEP);
                    if (read == 0)
                    {
                    }
                    else
                    {
                        data = server.m_udpBuf;

                        byte[] curPacket = new byte[2048];
                        Array.Copy(data, 0, curPacket, 0, read);

                        Invoke(RecShow, data[0].ToString());
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    BeginReceiveUDP(s, server);
                }
            }
        }

        /// <summary>
        /// Starts receiving UDP packets.
        /// </summary>
        /// <param name="s">Socket to receive packets.</param>
        /// <param name="server">Server instance used to receive packets.</param>
        private bool BeginReceiveUDP(Socket s, Form1 server)
        {
            bool ret = false;
            EndPoint tempRemoteEP;
            tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                s.BeginReceiveFrom(server.UDPBuffer, 0, m_udpBuf.Length, SocketFlags.None, ref tempRemoteEP, m_udpReceiveCallback, server);
                ret = true;
            }
            catch (Exception e)
            {
                throw (e);
            }

            return ret;
        }
    }
}
