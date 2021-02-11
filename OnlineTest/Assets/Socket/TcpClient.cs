using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
namespace GameSocket {
    public class ServiceState {
        public Socket serviceSocket;
        public byte[] buffer;
        public static int bufsize = 1024;
    }

    public class TcpClient {
        Socket c_socket;
        Thread th_socket;
        int ConnectionResult;
        string ip;
        int port;
        public TcpClient(string ip, int port) {
            this.ip = ip;
            this.port = port;
        }

        /// <summary>
        /// ����Socket
        /// </summary>
        public void Start() {
            try {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                c_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                c_socket.BeginConnect(endPoint, new AsyncCallback(Connect), c_socket);//���ӷ����
                th_socket = new Thread(MonitorSocker);//�����߳�
                th_socket.IsBackground = true;
                th_socket.Start();
            } catch (SocketException ex) {
                Debug.Log("error ex=" + ex.Message + " " + ex.StackTrace);
            }
        }

        public void Close() {
            try {
                c_socket.Shutdown(SocketShutdown.Both);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            try {
                c_socket.Close();
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        //����Socket
        void MonitorSocker() {
            while (true) {
                if (ConnectionResult != 0 && ConnectionResult != -2)//ͨ���������ж�
                {
                    Start();
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// ���ӷ����
        /// </summary>
        /// <param name="ar"></param>
        private void Connect(IAsyncResult ar) {
            try {
                ServiceState obj = new ServiceState();
                Socket client = ar.AsyncState as Socket;
                obj.serviceSocket = client;
                //��ȡ�������Ϣ
                client.EndConnect(ar);
                //��������Socket���� 
                client.BeginReceive(obj.buffer, 0, ServiceState.bufsize, SocketFlags.None, new AsyncCallback(ReadCallback), obj);
            } catch (SocketException ex) {
                ConnectionResult = ex.ErrorCode;
                Debug.Log(ex.Message + " " + ex.StackTrace);
            }
        }


        /// <summary>
        /// ���ݽ���
        /// </summary>
        /// <param name="ar">�����Socket</param>
        private void ReadCallback(IAsyncResult ar) {
            //��ȡ������
            ClientState obj = ar.AsyncState as ClientState;
            Socket c_socket = obj.clientSocket;
            int bytes = c_socket.EndReceive(ar);
            //������� ���¸���buffer����
            obj.buffer = new byte[ClientState.bufsize];
            c_socket.BeginReceive(obj.buffer, 0, ClientState.bufsize, 0, new AsyncCallback(ReadCallback), obj);
        }
    }
}