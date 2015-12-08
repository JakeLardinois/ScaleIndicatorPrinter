using System;
using Microsoft.SPOT;

using System.Net.Sockets;
using System.IO;


namespace Rinsen.WebServer.FileAndDirectoryServer
{
    public interface ISDCard
    {
        DirectoryInfo WorkingDirectoryInfo { get; set; }

        string GetWorkingDirectoryPath();

        void SendFile(string fullPath, Socket socket);

        //void RecieveFile(Socket socket);
        byte[] GetMoreBytes(Socket connectionSocket, out int count);

        bool Write(string path, string fileName, FileMode fileMode, string text);

        bool Write(string path, string fileName, FileMode fileMode, byte[] bytes, int length);

        string ReadTextFile(string fullPath);
    }
}
