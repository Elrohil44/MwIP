using Quantum;
using Quantum.Operations;
using System;
using System.Numerics;
using System.Collections.Generic;

namespace QuantumConsole
{
	public class QuantumTest
	{
		public static ulong gcd(ulong a, ulong b)
		{
			while (a != b && a != 0 && b != 0)
			{
				if (a > b)
				{
					a %= b;
				}
				else
				{
					b %= a;
				}
			}
			return a > 0 ? a : b;
		}
		
		public static long inverseModulo(long a, long n)
		{
			long t = 0, newT = 1, tmpT;
			long r = n, newR = a, tmpR;
		    long quotient;
			
			while (newR != 0) {
				quotient = r / newR;
				tmpR = newR;
				tmpT = newT;
				newR = r - quotient * newR;
				newT = t - quotient * newT;
				r = tmpR;
				t = tmpT;
			}
			
			if (r > 1) return -1;
			return t < 0 ? t + n : t;
		}
		
		public static Tuple<int, int> FractionalApproximation(int a, int b, int width)
        {
            double f = (double)a / (double)b;
            double g = f;
            int i, num2 = 0, den2 = 1, num1 = 1, den1 = 0, num = 0, den = 0;
            int max = 1 << width;

            do
            {
                i = (int)g;  // integer part
                g = 1.0 / (g - i);  // reciprocal of the fractional part

                if (i * den1 + den2 > max) // if denominator is too big
                {
                    break;
                }

                // new numerator and denominator
                num = i * num1 + num2;
                den = i * den1 + den2;

                // previous nominators and denominators are memorized
                num2 = num1;
                den2 = den1;
                num1 = num;
                den1 = den;

            }
            while (Math.Abs(((double)num / (double)den) - f) > 1.0 / (2 * max));
            // this condition is from Shor algorithm

            return new Tuple<int, int>(num, den);
        }
	
		public static int FindPeriod(int N, int a) {
			ulong ulongN = (ulong)N;
			int width = (int)Math.Ceiling(Math.Log(N, 2));
 
			Console.WriteLine("Width for N: {0}", width);
			Console.WriteLine("Total register width (7 * w + 2) : {0}", 7 * width + 2);
			
			QuantumComputer comp = QuantumComputer.GetInstance();
			
			//input register
			Register regX = comp.NewRegister(0, 2 * width);
			
			// output register (must contain 1):
			Register regX1 = comp.NewRegister(1, width + 1);
			
			// perform Walsh-Hadamard transform on the input register
			// input register can contains N^2 so it is 2*width long
			Console.WriteLine("Applying Walsh-Hadamard transform on the input register...");
			comp.Walsh(regX);
			
			// perform exp_mod_N
			Console.WriteLine("Applying f(x) = a^x mod N ...");
			comp.ExpModulo(regX, regX1, a, N);
			
			// output register is no longer needed
			regX1.Measure();
			
			// perform Quantum Fourier Transform on the input register
			Console.WriteLine("Applying QFT on the input register...");
			comp.QFT(regX);
			
			comp.Reverse(regX);
			
			// getting the input register
			int Q = (int)(1 << 2 * width);
			int inputMeasured = (int)regX.Measure();
			Console.WriteLine("Input measured = {0}", inputMeasured);
			Console.WriteLine("Q = {0}", Q);
			
			Tuple<int, int> result = FractionalApproximation(inputMeasured, Q, 2 * width - 1);
 
			Console.WriteLine("Fractional approximation:  {0} / {1}", result.Item1, result.Item2);
			
			int period = result.Item2;
			
			if(BigInteger.ModPow(a, period, N) == 1) {
				Console.WriteLine("Success !!!    period = {0}", period);
				return period;
			}
			
			int maxMult = (int)(Math.Sqrt(N)) + 1;
			int mult = 2;
			while(mult < maxMult) 
			{
				Console.WriteLine("Trying multiply by {0} ...", mult);
				period = result.Item2 * mult;
				if(BigInteger.ModPow(a, period, N) == 1) 
				{
					Console.WriteLine("Success !!!    period = {0}", period);
					return period;
				}
				else 
				{		
					mult++;
				}
			}
			
			Console.WriteLine("Failure !!!    Period not found, try again.");
			return -1;
		}
	
		public static int ExpModulo(int a, int x, int N)
		{
			// obliczamy ile bitow potrzeba na zapamiętanie N
			ulong ulongN = (ulong)N;
			int width = (int)Math.Ceiling(Math.Log(N, 2));
		 
			// inicjalizujemy komputer kwantowy 		
			QuantumComputer comp = QuantumComputer.GetInstance();
					
		 	//inicjalizujemy rejestr wejsciowy 
			Register regX = comp.NewRegister(0, 2 * width);
					
			// inicjalizujemy rejestr wyjsciowy 
			Register regY = comp.NewRegister(1, width + 1);
		 
			// ustawiamy wartosc rejestru wejsciowego na x 
			regX.Reset((ulong) x);
		 
			// ustawiamy wartosc rejestru wyjsciowego na 1
	        // potrzebne, gdy wywolujemy w petli 
			regY.Reset(1);
		 
	        // obliczamy a^x mod N
		 	comp.ExpModulo(regX, regY, a, N);
		 
	        //mierzymy wartosc
		 	return (int)regY.Measure();
		}
		
		public static void Main()
		{
			int N = 55;
			int b = 4;
			int c = 17;
			int r;
			int dPrim;
			int a = -1;
			
			Console.WriteLine("b = {0}", b);
			Console.WriteLine("N = {0}", N);
			Console.WriteLine();
			if (gcd((ulong) b, (ulong) N) == 1)
			{
				r = FindPeriod(N, b);
				
				Console.WriteLine("r = {0}", r);
				
				dPrim = (int) inverseModulo(c, r);
				
				Console.WriteLine("d' = {0}", dPrim);
				
				a = ExpModulo(b, dPrim, N);
				
				Console.WriteLine("a = {0}", a);
			}
		}
	}
}
