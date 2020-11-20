using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using IniParser;
using IniParser.Model;

namespace TRRP3Grpc
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("connection.ini");
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            using var channel = GrpcChannel.ForAddress("http://"+data["server"]["ip"]+":"+data["server"]["port"]);
            var client = new Quadratic.QuadraticClient(channel);
            Console.Write("Веб-сервис предназначен для решения квадратных уравнений вида ax^2+bx+c=0.\nВведите a: ");
            double operandA = Convert.ToDouble(Console.ReadLine());
            Console.Write("Введите b: ");
            double operandB = Convert.ToDouble(Console.ReadLine());
            Console.Write("Введите c: ");
            double operandC = Convert.ToDouble(Console.ReadLine());
            QuadraticResponse reply = await client.CalculateAsync(new QuadraticRequest { OperandA = operandA, OperandB = operandB, OperandC = operandC });
            if (reply.QuadraticStatus == QuadraticStatus.Ok)
                Console.WriteLine("Корни квадратного уравнения {0} и {1}", reply.Root1, reply.Root2);
            else
                Console.WriteLine("Нет вещественных корней");
        }
    }
}
