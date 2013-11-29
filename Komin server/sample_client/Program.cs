using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Komin;

namespace sample_client
{
    class Program
    {
        static KominClientSideConnection conn;

        static void Main(string[] args)
        {
            conn = new KominClientSideConnection();

            try
            {
                Console.Write("Podaj IP serwera: ");
                string IP = Console.ReadLine();
                Console.WriteLine("Łączę się z serwerem...");
                conn.Connect(IP, 666);
                Console.WriteLine("Podłączono: " + IP + ":666");

                string name, pass;
                string ch;
                do
                {
                    do
                    {
                        Console.Write("Chcesz utworzyć nowe konto? (tak/nie) "); ch = Console.ReadLine();
                    } while (ch.ToLower() != "tak" && ch.ToLower() != "nie");
                    if (ch.ToLower() == "tak")
                    {
                        Console.Write("Podaj login: "); name = Console.ReadLine();
                        Console.Write("Podaj haslo: "); pass = Console.ReadLine();
                        Console.WriteLine("Tworze konto " + name + " z hasłem " + pass + "...");
                        conn.CreateContact(name, pass);
                        Console.WriteLine("Stworzono konto");
                    }
                } while (ch.ToLower() != "nie");

                Console.Write("Podaj login: "); name = Console.ReadLine();
                Console.Write("Podaj haslo: "); pass = Console.ReadLine();
                Console.WriteLine("Loguje sie na konto {0} haslem {1}", name, pass);
                uint new_state = (uint)KominClientStatusCodes.Accessible;
                conn.Login(name, pass, ref new_state);
                Console.WriteLine("Zalogowano: twój contact_id to " + conn.userdata.contact_id);

                do
                {
                    do
                    {
                        Console.Write("0 - wyjscie, 1 - nasluchiwanie wiadomosci, 2 - wysylanie wiadomosci: ");
                        ch = Console.ReadLine();
                    } while (ch == "" || (ch[0] != '0' && ch[0] != '1' && ch[0] != '2'));
                    string msg = "";
                    switch (ch[0])
                    {
                        case '1':
                            Console.WriteLine("Czekam na wiadomosc...(naciśnij dowolny klawisz aby przerwać)");
                            conn.onNewTextMessage = odbiorca_wiadomosci;
                            while (conn.onNewTextMessage != null)
                            {
                                if (Console.KeyAvailable)
                                {
                                    Console.ReadKey(true);
                                    conn.onNewTextMessage = null;
                                }
                            }
                            break;
                        case '2':
                            Console.WriteLine("Podaj tresc wiadomosci:");
                            msg = Console.ReadLine();
                            Console.Write("Podaj contact_id odbiorcy: ");
                            conn.SendMessage(uint.Parse(Console.ReadLine()), false, msg);
                            break;
                    }
                } while (ch[0] != '0');
                


                Console.WriteLine("Wylogowywanie...");
                conn.Logout();
                Console.WriteLine("Wylogowano");
                Console.WriteLine("Rozłączanie...");
                conn.Disconnect();
                Console.WriteLine("Rozłączono");
                Console.WriteLine("(naciśnij dowolny klawisz aby zakończyć program...)"); Console.ReadKey(true);
            }
            catch (KominClientErrorException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
                return;
            }
        }

        static void odbiorca_wiadomosci(uint nadawca, uint odbiorca, bool czy_grupa, TextMessage wiadomosc)
        {
            if (odbiorca == conn.userdata.contact_id)
            {
                Console.WriteLine("wiadomosc od contact_id=" + nadawca + ": " + wiadomosc.message + " (wyslano: " + wiadomosc.send_date + ")");
                conn.onNewTextMessage = null;
            }
        }
    }
}
