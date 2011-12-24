# An Hashtable in PowerShell
$Dic = @{

	a = 1
	b = 2
	c = 3
}
function F1([int] $i1, [int] $i2) {	            

	return $i1 + $i2;
}
$Dic = @{

	a = 1  
	b = 2 
	c = (F1 2 1)
}