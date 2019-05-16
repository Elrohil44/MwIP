using Quantum;
using Quantum.Operations;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace QuantumConsole
{
	public static class GroverExtension
	{
		// Oracle is working on input register x (of width n)
		// and output register y (of width 1)
		public static void Oracle (this QuantumComputer comp, int target, Register x, Register y)
		{
			var controlBits = new RegisterRef[x.Width];
		
			for (int i = 0; i < x.Width; i++) 
			{
				if ((target & (1 << i)) == 0) 
				{
					x.SigmaX(i);
				}
				controlBits[i] = x[i];
			}
			
			comp.Toffoli(y[0], controlBits);	// Toffoli(<target_bit>, ... <control_bits> ...)
			
			for (int i = 0; i < x.Width; i++) 
			{
				if ((target & (1 << i)) == 0) 
				{
					x.SigmaX(i);
				}
			}
		}
		
		public static void InverseOracle (this QuantumComputer comp, int target, Register x, Register y)
		{
			comp.Oracle(target, x, y);
		}
		
		
		// Inversion is working on n-width register
		public static void Inversion (this QuantumComputer comp, Register x)
		{
			for (int i = 0; i < x.Width; i++) 
			{
				x.Hadamard(i);
				x.SigmaX(i);
			}
			
			x.Hadamard(0);
			
			if(x.Width == 2)
			{
				x.CNot(target: 0, control: 1);
			}
			else 
			{
				int[] controlBits = Enumerable.Range(1, x.Width - 1).ToArray();
				x.Toffoli(0, controlBits);		// Toffoli(<target_bit>, ... <control_bits> ...)
			}
			
			x.Hadamard(0);
			
			for (int i = 0; i < x.Width; i++) 
			{
				x.SigmaX(i);
				x.Hadamard(i);
			}
		}	
		
		public static void InverseInversion (this QuantumComputer comp, Register x)
		{
			comp.Inversion(x);
		}	
		
		
		
		// single iteration of Grover's algorithm
		public static void Grover (this QuantumComputer comp, int target, Register x, Register y)
		{
			comp.Oracle(target, x, y); 
  			comp.Inversion(x);
		}		
		
		public static void InverseGrover (this QuantumComputer comp, int target, Register x, Register y)
		{
			comp.Inversion(x);
			comp.Oracle(target, x, y); 
		}
	}

	public class QuantumTest
	{
	
		public static void Main()
		{
			int number = 5;
			int width = 4;	// width of search space
			double iterations = Math.PI/4 * Math.Sqrt(1 << width);
		
			QuantumComputer comp = QuantumComputer.GetInstance();
			
			// input register set to 0
			Register x = comp.NewRegister(0, width);
			
			// output 1-qubit-register, set to 1
			Register y = comp.NewRegister(1, 1);

			comp.Walsh(x);
			comp.Hadamard(y[0]);
			
			Console.WriteLine("Iterations needed: PI/4 * Sqrt(2^n) = {0}", iterations);

			for (int i = 1; i <= iterations; i++)
			{
				Console.WriteLine("Iteration #{0}", i);
				comp.Grover(number, x, y);
			}
		}
	}
}
