using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.Threading;
namespace GameSocket {
    public class ClientState {
        public Socket clientSocket;
        public byte[] buffer;
        public static int bufsize = 1024;
    }

    public class TcpServer {
        Socket s_socket;
        Thread th_socket;
        public TcpServer(string ip, int port) {
            // ����һ��socket
            s_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // ��IP��ַ�Ͷ˿�
            s_socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            // ��ʼ����
            Debug.Log("��������ʼ����");
            s_socket.Listen(100); // ��������������

            th_socket = new Thread(() => { s_socket.BeginAccept(new AsyncCallback(Accept), s_socket); });//�����߳�
            th_socket.IsBackground = true;
            th_socket.Start();
        }

        public void Close() {
            try {
                s_socket.Shutdown(SocketShutdown.Both);
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
            try {
                s_socket.Close();
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// �첽���ӻص� ��ȡ����Socket
        /// </summary>
        /// <param name="ar">�����Socket</param>
        private void Accept(IAsyncResult ar) {
            try {
                //��ȡ����Socket �����µ�����
                Socket myServer = ar.AsyncState as Socket;
                Socket service = myServer.EndAccept(ar);

                ClientState obj = new ClientState();
                obj.clientSocket = service;
                //��������Socket����
                service.BeginReceive(obj.buffer, 0, ClientState.bufsize, SocketFlags.None, new AsyncCallback(ReadCallback), obj);
                myServer.BeginAccept(new AsyncCallback(Accept), myServer);//�ȴ���һ������
            } catch (Exception ex) {
                Console.WriteLine("����˹ر�" + ex.Message + " " + ex.StackTrace);
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

        /// <summary>
        /// ������Ϣ
        /// </summary>
        /// <param name="c_socket">ָ���ͻ���socket</param>
        /// <param name="by">��������</param>
        private void Send(Socket c_socket, byte[] by) {
            //����
            c_socket.BeginSend(by, 0, by.Length, SocketFlags.None, asyncResult => {
                try {
                    //�����Ϣ����
                    int len = c_socket.EndSend(asyncResult);
                } catch (Exception ex) {
                    Console.WriteLine("error ex=" + ex.Message + " " + ex.StackTrace);
                }
            }, null);
        }
    }
}