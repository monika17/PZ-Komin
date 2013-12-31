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
            conn.onStatusNotification = onStatusNote;
            conn.onServerLogout = onServerLogout;

            try
            {
                Console.Write("Podaj IP serwera: ");
                string IP = Console.ReadLine();
                Console.WriteLine("Łączę się z serwerem...");
                conn.Connect(IP, 8888);
                Console.WriteLine("Podłączono: " + IP + ":8888");

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

                Console.WriteLine();
                Console.WriteLine("Zaloguj sie");
                Console.Write("Podaj login: "); name = Console.ReadLine();
                Console.Write("Podaj haslo: "); pass = Console.ReadLine();
                Console.WriteLine("Loguje sie na konto {0} haslem {1}", name, pass);
                uint new_state = (uint)KominClientStatusCodes.Accessible;
                conn.Login(name, pass, ref new_state);
                Console.WriteLine("Zalogowano: twój contact_id to " + conn.userdata.contact_id);

                do
                {
                    Console.Write("0 - wyjscie\n1 - nasluchiwanie wiadomosci\n2 - wysylanie wiadomosci\n3 - ustaw status\n");
                    do
                    {
                        Console.Write(": ");
                        ch = Console.ReadLine();
                    } while (ch == "" || (int.Parse(ch) < 0 || int.Parse(ch) > 3));
                    string msg = "";
                    switch (int.Parse(ch))
                    {
                        case 1:
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
                        case 2:
                            Console.WriteLine("Podaj tresc wiadomosci:");
                            msg = Console.ReadLine();
                            Console.Write("Podaj contact_id odbiorcy: ");
                            conn.SendMessage(uint.Parse(Console.ReadLine()), false, msg);
                            break;
                        case 3:
                            Console.Write("Podaj nowy status (0-3): "); uint new_status = uint.Parse(Console.ReadLine());
                            conn.SetStatus(new_status);
                            break;
                    }
                } while (int.Parse(ch) != 0);



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
                Console.WriteLine("(naciśnij dowolny klawisz aby zakończyć program...)"); Console.ReadKey(true);
                conn.Disconnect();
                return;
            }
            catch (Exception) { }
        }

        private static void onServerLogout()
        {
            Console.WriteLine("\nSerwer wylogowal uzytkownika");
            Console.WriteLine("Rozłączanie...");
            conn.Disconnect();
            Console.WriteLine("Rozłączono");
            Console.WriteLine("(naciśnij dowolny klawisz aby zakończyć program...)"); Console.ReadKey(true);
            throw new Exception();
        }

        private static void onStatusNote(ContactData changed_contact)
        {
            Console.WriteLine("\nKontakt {0} o nicku {2} zmienil status na {1}", changed_contact.contact_id, changed_contact.status, changed_contact.contact_name);
        }

        static void odbiorca_wiadomosci(uint nadawca, uint odbiorca, bool czy_grupa, TextMessage wiadomosc)
        {
            if (odbiorca == conn.userdata.contact_id)
            {
                Console.WriteLine("wiadomosc od contact_id=" + nadawca + ": " + wiadomosc.message + " (wyslano: " + wiadomosc.send_date + ")");
            }
        }
    }
}
