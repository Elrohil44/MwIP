using Quantum;
using Quantum.Operations;
using System;
using System.Numerics;
using System.Collections.Generic;

namespace QuantumConsole
{
	public class QuantumTest
	{
		public static void Main()
		{
			QuantumComputer comp = QuantumComputer.GetInstance();

			int a = 25;
			int n = (int) Math.Ceiling(Math.Log(a, 2));
			// create new register with initial value = 0, and width = 3 
			Register x = comp.NewRegister(1, n + 1);
			
			// Quantum solution
			for (int i = 0; i <= n; i++) x.Hadamard(i);

			for (int i = 1, tmp = a; i <= n; i++)
			{
				if (tmp % 2 == 1)
				{
					x.CNot(target: 0, control: i);
				}
				tmp /= 2;
			}
			
			
			// Quantum solution
			for (int i = 0; i <= n; i++) x.Hadamard(i);
		}
	}
}
