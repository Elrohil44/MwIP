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
			x.Hadamard(1);
			x.CNot(target: 0, control: 1);
		}
	}
}
