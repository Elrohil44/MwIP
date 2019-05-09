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
			Register x = comp.NewRegister(0, 2);
			x.SigmaX(0);
			x.SigmaX(1);
			x.Hadamard(0);
			x.Hadamard(1);
			
			// f_00
			//
			
			// f_01
			// x.CNot(target: 0, control: 1);
			
			// f_10
			// x.SigmaX(0);
			// x.CNot(target: 0, control: 1);
			
			// f_11
			// x.SigmaX(0);
				
			x.Hadamard(1);
		}
	}
}
