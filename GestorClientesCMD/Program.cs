using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GestorClientesCMD
{
    class Program
    {
        enum Menu { Adicionar = 1, Listar, Remover, Sair = 0 }

        enum MenuExcluir { ExcluirPorId=1, ExcluirPorNome, ExcluirTodos, Retornar = 0 }

        [System.Serializable]
        struct Cliente
        {
            public string Nome;
            public string Email;
            public string CPF;
        }

        static List<Cliente> clientes = new List<Cliente>();

        static void Main()
        {
            LerClientes();

            bool rodar = true;

            while (rodar)
            {
                Console.WriteLine("Gestor de clientes");
                Console.WriteLine("------------------");
                Console.WriteLine("1 - Adicionar\n2 - Listar\n3 - Remover\n0 - Sair");

                if (int.TryParse(Console.ReadLine(), out int escolha))
                {
                    Menu opcao = (Menu)escolha;
                    switch (opcao)
                    {
                        case Menu.Listar:
                            if (clientes.Count != 0)
                            {
                                ListarClientes();
                                Console.Clear();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Não há clientes cadastrados");
                                Thread.Sleep(1000);
                                Console.ResetColor();
                                Console.Clear();
                            }

                            break;
                        case Menu.Adicionar:
                            Console.Clear();
                            AdicionarCliente();
                            break;
                        case Menu.Remover:
                            if(clientes.Count != 0)
                            {
                                Console.Clear();
                                RemoverCliente();
                                Console.Clear();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Não há clientes cadastrados");
                                Thread.Sleep(1000);
                                Console.ResetColor();
                                Console.Clear();
                            }

                            break;
                        case Menu.Sair:
                            rodar = false;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERRO: Selecione uma opção válida");
                            Console.ResetColor();
                            Thread.Sleep(1000);
                            Console.Clear();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERRO: Selecione uma opção válida");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    Console.Clear();
                }
            }
            Console.Clear();

        }
        static void AdicionarCliente()
        {
            Cliente cliente = new Cliente();

            Console.WriteLine("Adicionar cliente");
            Console.WriteLine("-----------------");

            Console.Write("Nome: ");
            cliente.Nome = Console.ReadLine();

            Console.Write("Email: ");
            cliente.Email = Console.ReadLine();

            Console.Write("CPF: ");
            cliente.CPF = Console.ReadLine();

            clientes.Add(cliente);

            ExportarClientes();

            Console.WriteLine();
            Console.WriteLine("Cliente adicionado com sucesso!");

            Thread.Sleep(1000);

            Console.Clear();
        }
        static void ListarClientes()
        {
            int indice = 0;
            Console.WriteLine("----------------------------------------------------------------------------------------");
            foreach (Cliente c in clientes)
            {
                Console.WriteLine($"{indice}) Nome: {c.Nome} | Email: {c.Email} | CPF: {c.CPF}");
                Console.WriteLine("----------------------------------------------------------------------------------------");
                indice++;
            }

            Console.WriteLine("Pressione ENTER para retornar...");
            Console.ReadLine();
        }
        static void ExportarClientes()
        {
            FileStream stream = new FileStream("clientes.bin", FileMode.OpenOrCreate);
            BinaryFormatter encoder = new BinaryFormatter();

            encoder.Serialize(stream, clientes);

            stream.Close();
        }
        static void LerClientes()
        {
            FileStream stream = new FileStream("clientes.bin", FileMode.OpenOrCreate);

            try
            {
                BinaryFormatter encoder = new BinaryFormatter();

                clientes = (List<Cliente>)encoder.Deserialize(stream);

                if(clientes == null)
                    clientes = new List<Cliente>();

            }
            catch
            {
                clientes = new List<Cliente>();
            }

            stream.Close();
        }
        static void RemoverCliente()
        {
            Console.WriteLine("Remover cliente");
            Console.WriteLine("---------------");

            Console.WriteLine("1 - Remover por id\n2 - Remover por nome\n3 - Remover todos\n0 - Retornar");

            if (int.TryParse(Console.ReadLine(), out int escolha))
            {
                MenuExcluir opcao = (MenuExcluir)escolha;
                switch(opcao)
                {
                    case MenuExcluir.ExcluirPorNome:
                        ExcluirPorNome();
                        break;
                    case MenuExcluir.ExcluirTodos:
                        ExcluirTodos();
                        break;

                    case MenuExcluir.ExcluirPorId:
                        ExcluirPorId();
                        break;

                    case MenuExcluir.Retornar:
                        break;
                }
            }

        }
        static void ExcluirPorNome()
        {
            Console.Write("Informe o nome do cliente: ");
            string nome = Console.ReadLine();

            int quantidade = clientes.RemoveAll(clientes => clientes.Nome == nome);
            if (quantidade > 0)
            {
                Console.WriteLine($"{quantidade} clientes removidos");
                Thread.Sleep(1000);
                ExportarClientes();
                LerClientes();
            }
            else
            {
                Console.WriteLine("Nenhum cliente encontrado com o nome recebido");
                Thread.Sleep(1000);
            }
        }
        static void ExcluirPorId()
        {
            Console.Write("Informe a Id do cliente a ser removido: ");

            if(!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Digite um valor válido");
                Thread.Sleep(1000);
                return;
            }

            bool sucesso = false;
            try
            {
                clientes.RemoveAt(id);
                sucesso = true;
            }   
            catch
            {
                Console.WriteLine("Não foi possível remover o cliente");
            }
            finally
            {
                if(sucesso)
                    Console.WriteLine($"Cliente id: {id} removido com sucesso");
            }
            Thread.Sleep(1000);
        }
        static void ExcluirTodos()
        {
            Console.Write("Essa ação irá apagar o arquivo de registro. Continuar? (S|N): ");
            string simnao = Console.ReadLine() ?? "N";

            switch (simnao)
            {
                case "S":
                case "s":

                    File.Delete("clientes.bin");
                    clientes.Clear();
                    Console.WriteLine("Clientes removidos com sucesso");
                    Thread.Sleep(1000);

                    break;

                default:
                    break;
            }
        }
    }
}
