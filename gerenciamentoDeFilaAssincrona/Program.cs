using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GerenciamentoDeFilaAssincrona
{
    class Program
    {
        //SemaphoreSlim serve para controlar o fluxo de Threads, só permitindo no maximo 3 ser executada. conforme sao finalizadas, slots sao liberados.
        static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(3);
        // fila intanciada para armazenar threads se o semaphoro estiver lotado
        static Queue<Thread> queue = new Queue<Thread>();
        // Contador para enumerar de forma crescente o numero de thread cridas e a para aparecer no log
        static int counter = 1;
        static void Main(string[] args)
        {
            Console.WriteLine("type SpaceBar to initialize a Thread Simulation: ");

            // Fluxo para manter a aplicação rodando, o console faz a leitura do input do usuario. se for spaceBbar as açoes são iniciadas.
            while (true)
            {

                var keyTyped = Console.ReadKey();

                if (keyTyped.Key == ConsoleKey.Spacebar || queue.Count > 0)
                {
                    //condição que verifica se o semaforo esta não lotado, lembrando que apenas 3 espaços ppodem ser executados.
                    if (_semaphoreSlim.CurrentCount != 0)
                    {
                        // se a tiver threads na fila, o fluxo remove a primeira da filça e executa
                        if (queue.Count > 0)
                        {
                            Thread threadFromQueue = queue.Dequeue();
                            threadFromQueue.Start();

                        }
                        // se não a thread é criada diretamente e executada
                        else
                        {
                            Thread thread = new Thread(ThreadSimulation);
                            thread.Name = "Thread : " + counter;
                            thread.Start();
                            counter++;
                        }
                    }
                    // se o semaforo estiver cheio, o fluxo entra nessa condição para criar threads  e adcionar na fila para posterior execução.
                    else
                    {
                        Thread thread = new Thread(ThreadSimulation);
                        thread.Name = "Thread : " + counter;
                        queue.Enqueue(thread);
                        counter++;
                        Console.WriteLine("queue count" + queue.Count);
                    }

                }
            }
        }

        //Metodo para simular uma thread, pode ser chamada varias vezes, desde que respeite o semaphoro. O metodo Wait() sinaliza que a thread nao pode ser chamada. só pode ser quando o Release() for executado.
        public static void ThreadSimulation()
        {
            // a thread simula uma operação de 3 segundos
            _semaphoreSlim.Wait();
            Console.WriteLine($"The {Thread.CurrentThread.Name} has begin at {DateTime.Now:HH:mm:ss.fff}");
            Thread.Sleep(3000);
            Console.WriteLine($"The {Thread.CurrentThread.Name} has been finished at {DateTime.Now:HH:mm:ss.fff}");
            // após o encerramento da simulação, o slot ainda fica preenchido por 10 segundos, para ai entao ser executado.
            Thread.Sleep(10000);
            _semaphoreSlim.Release();

        }
    }
}
