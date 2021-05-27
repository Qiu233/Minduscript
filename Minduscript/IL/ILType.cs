using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minduscript.IL
{
	public enum ILType
	{
		Nop,
		Using,//using $a as $b $ $
		Set,//$a = $b $ $
		Assignment,//$a = $b $c $op
		Param,//param $a $ $ $

		Mem_Read,//mem_read $var $mem $pos $
		Mem_Write,//mem_write $var $mem $pos $

		End,//end $ $ $ $
		Call,//call $retV $macro $ $
		Jmp,//jmp $target $ $ $
		Je,//je $target $a $b $
		Jne,
		Jl,
		Jle,
		Jg,
		Jge,
		Jse,
	}
}
