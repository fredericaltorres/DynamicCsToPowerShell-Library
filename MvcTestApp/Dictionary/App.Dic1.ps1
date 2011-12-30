# Dictionary must start with the line
function F1([double] $i1, [double] $i2) {
	return $i1 + $i2;
}
$Dic = @{
	ApplicationTitle = 'My App Title ***'
	Amount = (F1 2.2 1.1)
}
