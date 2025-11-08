using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RestauranteConsola
{
    class Program
    {
        // Clase para representar un platillo
        class Platillo
        {
            public string Nombre { get; set; }
            public decimal Precio { get; set; }

            public Platillo(string nombre, decimal precio)
            {
                Nombre = nombre;
                Precio = precio;
            }
        }

        // Clase para representar un pedido (platillo + cantidad)
        class Pedido
        {
            public Platillo Platillo { get; set; }
            public int Cantidad { get; set; }

            public decimal Subtotal => Platillo.Precio * Cantidad;
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Para símbolo de $
            var currencyCulture = CultureInfo.GetCultureInfo("es-MX"); // Formato de moneda (peso mexicano)
            List<Platillo> menu = new List<Platillo>()
            {
                new Platillo("Tacos al pastor", 80m),
                new Platillo("Enchiladas verdes", 75m),
                new Platillo("Pozole", 80m),
                new Platillo("Tostadas de pollo", 65m),
                new Platillo("Agua fresca", 30m)
            };

            List<Pedido> pedidos = new List<Pedido>();
            bool continuar = true;

            Console.WriteLine("🧙‍♂ Bienvenido al Restaurante 'El caldero chorreante'");

            while (continuar)
            {
                Console.WriteLine("Menú del día:");
                for (int i = 0; i < menu.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {menu[i].Nombre} - {menu[i].Precio.ToString("C", currencyCulture)}");
                }

                int opcion = ReadInt($"Seleccione el número del platillo que desea pedir (1-{menu.Count}): ", 1, menu.Count) - 1;
                int cantidad = ReadInt($"¿Cuántas órdenes de {menu[opcion].Nombre} desea?: ", 1, int.MaxValue);

                // Combinar si ya existe el mismo platillo en pedidos
                var existente = pedidos.FirstOrDefault(p => p.Platillo == menu[opcion]);
                if (existente != null)
                {
                    existente.Cantidad += cantidad;
                }
                else
                {
                    pedidos.Add(new Pedido { Platillo = menu[opcion], Cantidad = cantidad });
                }

                continuar = ReadYesNo("\n¿Desea agregar otro platillo? (s/n): ");
            }

            if (pedidos.Count == 0)
            {
                Console.WriteLine("\nNo se realizaron pedidos. Hasta luego.");
                return;
            }

            // Calcular total
            decimal total = pedidos.Sum(p => p.Subtotal);

            // Preguntar por propina
            bool deseaPropina = ReadYesNo("\n¿Desea dejar propina? (s/n): ");
            decimal propina = 0m;
            if (deseaPropina)
            {
                decimal porcentaje = ReadDecimal("Ingrese el porcentaje de propina (%): ", 0m, 1000m);
                propina = decimal.Round(total * (porcentaje / 100m), 2);
            }

            decimal totalFinal = decimal.Round(total + propina, 2);

            // Imprimir ticket
            Console.Clear();
            Console.WriteLine("===================================");
            Console.WriteLine("     🧾 Ticket de compra 🧾");
            Console.WriteLine("===================================");
            Console.WriteLine("Platillos pedidos:");

            foreach (var p in pedidos)
            {
                Console.WriteLine($"- {p.Platillo.Nombre} x{p.Cantidad}  @ {p.Platillo.Precio.ToString("C", currencyCulture)}  = {p.Subtotal.ToString("C", currencyCulture)}");
            }

            Console.WriteLine("-----------------------------------");
            Console.WriteLine($"Subtotal: {total.ToString("C", currencyCulture)}");
            Console.WriteLine($"Propina:  {propina.ToString("C", currencyCulture)}");
            Console.WriteLine($"TOTAL A PAGAR: {totalFinal.ToString("C", currencyCulture)}");
            Console.WriteLine("===================================");
            Console.WriteLine("Gracias por su compra (👉 ﾟヮﾟ) 👉");
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Entrada vacía, intente de nuevo.");
                    continue;
                }
                if (int.TryParse(input.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out int value))
                {
                    if (value < min || value > max)
                    {
                        Console.WriteLine($"Valor fuera de rango ({min} - {max}).");
                        continue;
                    }
                    return value;
                }
                Console.WriteLine("Entrada inválida, ingrese un número entero.");
            }
        }

        static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Entrada vacía, intente de nuevo.");
                    continue;
                }

                // Intentar con cultura actual y con invariante para aceptar coma o punto
                if (decimal.TryParse(input.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal value) ||
                    decimal.TryParse(input.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    if (value < min || value > max)
                    {
                        Console.WriteLine($"Valor fuera de rango ({min} - {max}).");
                        continue;
                    }
                    return value;
                }
                Console.WriteLine("Entrada inválida, ingrese un número válido.");
            }
        }

        static bool ReadYesNo(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Entrada vacía, intente de nuevo.");
                    continue;
                }
                var low = input.Trim().ToLowerInvariant();
                if (low == "s" || low == "si")
                    return true;
                if (low == "n" || low == "no")
                    return false;
                Console.WriteLine("Respuesta inválida, responda 's' o 'n'.");
            }
        }
    }
}