using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace TrabalhoPDC
{
    class Program
    {
        
        static void Main(string[] args)
        {
            bool programa = true;

            ThreadVars varGastadora = new ThreadVars();
            ThreadVars varEsperta = new ThreadVars();
            ThreadVars varEconomica = new ThreadVars();
            ThreadVars varPatrocinadora = new ThreadVars();

            Conta contaFamilia = new Conta(1000.00);

            Cliente AGastadora = new Cliente("AGastadora");
            Cliente AEsperta = new Cliente("AEsperta");
            Cliente AEconomica = new Cliente("AEconomica");
            Cliente APatrocinadora = new Cliente("APatrocinadora");

         
            Thread Tpatrocinadora = new Thread(() =>
            {
                varPatrocinadora.contador = 0;
                varPatrocinadora.saldo = 0.00;
                while (programa)
                {
                    if(contaFamilia.Saldo == 0)
                    {
                        contaFamilia.Depositar(APatrocinadora, 100.00);
                        varPatrocinadora.saldo += 100;
                        varPatrocinadora.contador++;
                    }
                }
            });
            Thread Tgastadora = new Thread(() =>
            {
                varGastadora.contador = 0;
                varGastadora.saldo = 0.00;
                Thread.Sleep(1500);
                while (programa)
                {
                    
                    Thread.Sleep(3000);
                    varGastadora.saldo += contaFamilia.Sacar(AGastadora, 10.00);
                    varGastadora.contador++;  
                }
            });
            Thread Tesperta = new Thread(() =>
            {
                varEsperta.contador = 0;
                varEsperta.saldo = 0.00;
                Thread.Sleep(1500);
                while (programa)
                {
                    
                    Thread.Sleep(6000);
                    varEsperta.saldo += contaFamilia.Sacar(AEsperta, 50.00);
                    varEsperta.contador++;
                }
            });
            Thread Teconomica = new Thread(() =>
            {
                varEconomica.contador = 0;
                varEconomica.saldo = 0.00;
                Thread.Sleep(1500);
                while (programa)
                {
                    
                    Thread.Sleep(12000);
                    varEconomica.saldo += contaFamilia.Sacar(AEconomica, 5.00);
                    varEconomica.contador++;                               
                }
            });
            Console.WriteLine("Precione qualquer tecla e depois 'Enter' para parar o programa.\n");

            Tgastadora.Start();
            Tesperta.Start();
            Teconomica.Start();
            Tpatrocinadora.Start();

            while (programa)
            {
                var tecla = Console.ReadLine() + "\n";
                if (tecla != null)
                {
                    programa = false;
                }
            }
            while (Tgastadora.IsAlive == true || Tesperta.IsAlive == true || Teconomica.IsAlive == true || Tpatrocinadora.IsAlive == true)
            {

            }
            Console.WriteLine("\n");
            Console.WriteLine("APatrocinadora realizou {0} operacoes com o valor total de ${1}", varPatrocinadora.contador, varPatrocinadora.saldo);
            Console.WriteLine("AGastadora realizou {0} saques com o valor total de ${1}", varGastadora.contador, varGastadora.saldo);
            Console.WriteLine("AEsperta realizou {0} saques com o valor total de ${1}", varEsperta.contador, varEsperta.saldo);
            Console.WriteLine("AEconomica realizou {0} saques com o valor total de ${1}", varEconomica.contador, varEconomica.saldo);
           
            
        }
    }
}
struct ThreadVars
{
    public int contador;
    public double saldo;
}
class Conta
{
    public double Saldo {get; set;}

    public Conta(double saldo)
    {
        Saldo = saldo;
    }

    public double Sacar(Cliente cliente, double saque)
    {
        lock (cliente)
        {
            while(Saldo == 0.00)
            {
                Monitor.Wait(cliente);
            }
            if ((Saldo - saque) < 0)
            {
                Console.WriteLine("{0} sacou ${1}, deixando na Conta:$0.00", cliente.Nome, Saldo);
                double valorRestante = Saldo;
                Saldo -= Saldo;
                return valorRestante;
            }
            if (Saldo >= saque)
            {
                Console.WriteLine("{0} sacou ${1}, deixando na Conta:${2}", cliente.Nome, saque, (Saldo - saque));
                Saldo -= saque;
                return saque;
            }
            return Saldo;
        }     
    }

    public void Depositar(Cliente cliente, double deposito)
    {
        lock(cliente)
        {
            Console.WriteLine("-{0}- depositou $100.00 na Conta", cliente.Nome);
            Saldo += deposito;
            Monitor.Pulse(cliente);
        }
        
    }
}

class Cliente
{
    public string Nome { get; set; }
    public Cliente(string nome) 
    {
        Nome = nome;
    }
}
