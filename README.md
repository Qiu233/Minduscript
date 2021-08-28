# Minduscript
Minduscript is a structured programming language, which can be compiled into Mindustry processor assembly code.

# Overview
As you will see, Minduscript code looks like js.  
But the main difference is that Minduscript's function does not support recursive calling.  
Here's an example of minduscript:

```js
assembly test; ? declare the assembly name

function main() ? the entry function
{
	using bank1,display1; ? using outer component from game
	var n=50; ? local variable
	bank1[1]=1; ? accessing memory component
	bank1[2]=1;
	for(var i=3;i<=n;i=i+1) ? for loop
	{
		bank1[i]=bank1[i-1]+bank1[i-2];
	}
	# print bank1[n] # ? inline asm code
	# printflush display1 #
}
```
