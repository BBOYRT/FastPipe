using System;
using System.IO;
using System.IO.Pipes;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;

namespace PIPE_LIB
{
    public class Client
    {
        StreamReader reader;
        StreamWriter writer;
        bool Connect = false;
        NamedPipeClientStream pipeClient;
        string PipeName_;
        Queue<string> read_Queue = new Queue<string>();
        Queue<string> send_Queue = new Queue<string>();
        public string init_Client(string pipeName)
        {
            if (pipeName != null)
            {
                if (pipeClient == null)
                {
                    PipeName_ = pipeName;
                    return "ok";
                }
                else
                {
                    return "please first deinit";
                }
            }
            else
            {
                return "input is null";
            }
        }
        public string Begin_Client()
        {
            if (PipeName_ != null)
            {
                Task.Run(wait_Connect);
                Task.Run(Client_send);
                Task.Run(Client_read);
                return "ok";
            }
            else
            {
                return "init_server not run";
            }
        }
        public void send_data(string DATA)
        {
            send_Queue.Enqueue(DATA);
        }
        public string read_data()
        {
            if (read_Queue.Count > 0)
            {
                return read_Queue.Dequeue();
            }
            else
            {
                return "";
            }
        }
        public string ckick_Connect()
        {
            if (pipeClient != null)
            {
                return Connect.ToString();
            }
            else
            {
                return "init_server not run";
            }
        }
        private async Task wait_Connect()
        {
            while (true)
            {
                if (pipeClient != null && pipeClient.IsConnected)
                {
                    if (reader == null || writer == null)
                    {
                        reader = new StreamReader(pipeClient);
                        writer = new StreamWriter(pipeClient) { AutoFlush = true };
                    }
                    Connect = true;
                    await Task.Delay(100);
                }
                else
                {
                    Connect = false;
                    reader = null;
                    writer = null;
                    try
                    {
                        pipeClient = new NamedPipeClientStream(".", PipeName_, PipeDirection.InOut, PipeOptions.Asynchronous);
                        pipeClient.Connect();
                    }
                    finally { }
                    await Task.Delay(200);
                }
            }
        }
        private async Task Client_send()
        {
            while (true)
            {
                if (pipeClient != null)
                {
                    if (send_Queue.Count > 0 && pipeClient.IsConnected && writer != null)
                    {
                        lock (send_Queue)
                        {
                            writer.WriteLineAsync(send_Queue.Dequeue());
                        }
                    }
                }
                await Task.Delay(10);
            }
        }
        private async Task Client_read()
        {
            while (true)
            {
                if (pipeClient != null)
                {
                    if (pipeClient.IsConnected && reader != null)
                    {
                        string line = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lock (read_Queue)
                            {
                                read_Queue.Enqueue(line);
                            }
                        }
                    }
                }
                await Task.Delay(10);
            }
        }
    }
    public class Server
    {
        NamedPipeServerStream server = null;
        bool Connect = false;
        StreamWriter writer = null;
        StreamReader Reader = null;
        string pipe_name_;
        Queue<string> READ_DATA = new Queue<string>();
        Queue<string> SEND_DATA = new Queue<string>();
        public string init_server(string pipe_name)
        {
            if (pipe_name != null)
            {
                if (server == null)
                {
                    pipe_name_ = pipe_name;
                    return "ok";
                }
                else
                {
                    return "please first deinit";
                }
            }
            else
            {
                return "input is null";
            }
        }
        public string Begin_server()
        {
            if (pipe_name_ != null)
            {
                Task.Run(wait_Connect);
                Task.Run(server_send);
                Task.Run(server_read);
                return "ok";
            }
            else
            {
                return "init_server not run";
            }
        }
        public string ckick_Connect()
        {
            if (server != null)
            {
                return Connect.ToString();
            }
            else
            {
                return "init_server not run";
            }
        }
        public void send_data(string DATA)
        {
            SEND_DATA.Enqueue(DATA);
        }
        public string read_data()
        {
            if (READ_DATA.Count > 0)
            {
                return READ_DATA.Dequeue();
            }
            else
            {
                return "";
            }
        }
        private async Task wait_Connect()
        {
            server = new NamedPipeServerStream(pipe_name_, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            server.WaitForConnection();
            while (true)
            {
                if (server.IsConnected)
                {
                    if (Reader == null || writer == null)
                    {
                        Reader = new StreamReader(server);
                        writer = new StreamWriter(server) { AutoFlush = true };
                    }
                    Connect = true;
                    await Task.Delay(100);
                }
                else
                {
                    Connect = false;
                    Reader = null;
                    writer = null;
                    server = new NamedPipeServerStream(pipe_name_, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    server.WaitForConnection();
                    await Task.Delay(10);
                }
            }
        }
        private async Task server_send()
        {
            while (true)
            {
                if (server != null)
                {
                    if (SEND_DATA.Count > 0 && server.IsConnected && writer != null)
                    {
                        lock (SEND_DATA)
                        {
                            writer.WriteLineAsync(SEND_DATA.Dequeue());
                        }
                    }
                }
                await Task.Delay(10);
            }
        }
        private async Task server_read()
        {
            while (true)
            {
                if (server != null)
                {
                    if (server.IsConnected && Reader != null)
                    {
                        string line = await Reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(line))
                        {
                            lock (READ_DATA)
                            {
                                READ_DATA.Enqueue(line);
                            }
                        }
                    }
                }
                await Task.Delay(10);
            }
        }
    }
}
