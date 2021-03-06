param ($A = @(5,4,3,2,1,0))
#Counting Inversions
function SortAndCount($A, $length)
{
    #Base case when the array is of size 2
    if ($length -eq 1)
    {
        return 0, $A
    }
    else
    {
        #Recursively count left, right and split inversions
        $Left = FirstHalf($A)
        $Right = SecondHalf($A)
        $X = @()
        $Y = @()
        
        $m = ([Math]::Floor($length / 2))
                
        ### SORT SUB ARRAYS ###
        #First recursive call to sort and count the first half of the array
        $X += SortAndCount $Left $m
        
        #When the first half is sorted using the first recursive call, second
        #resursive call to sort the second half of the array
        $Y += SortAndCount $Right ($A.Length - $m)
        ### SORT SUB ARRAYS ###
                
        #When both recursive calls finish, counting the split inversions by piggybacking on merge sort
        $Z += MergeAndCountSplitInv $X[1] $Y[1] $length
        
        return ($X[0] + $Y[0] + $Z[0]), $Z[1]
    }
}

function MergeAndCountSplitInv([array]$L, [array]$R, $n)
{
    $Count = 0
    $tempArray = @()
    
    #current pointers to each list
    $i = 0; $j = 0;
    
    for ($k = 0; $k -lt $n; $k++)
    {
        if (($L[$i] -lt $R[$j]) -and ($L[$i] -ne $null))
        {
            $tempArray += $L[$i]
            $i++
        }
        elseif (($R[$j] -lt $L[$i]) -and ($R[$j] -ne $null))
        {
            $tempArray += $R[$j]
            # Count inversions
            $Count += $L.Length - $i
            
            $j++
            
        }
        
    }
    # Copy the remaining list to C
    if ($j -lt $i)
    {
        for ($t = $j; $t -lt $R.Length; $t++)
        {
            $tempArray += $R[$t]
        }
    }
    else
    {
        for ($t = $i; $t -lt $L.Length; $t++)
        {
            $tempArray += $L[$t]
        }
    }
    
    return $Count, $tempArray
}

function FirstHalf([array]$a)
{
    if ($a.Length -eq 1)
    {return}
    
    #write-host "inside FirstHalf()"
    $mid  = [Math]::Floor(($a.Length) / 2)
    $firstHalf = @()
    0..($mid - 1) | %{ $firstHalf += $a[$_] }
    
    #Find left inversions
    $count = 0
    0..(($firstHalf.Length) - 1) | %{
        for ($i = $_; $i -lt $firstHalf.Length; $i++)
        {
            if ($firstHalf[$_] -gt $firstHalf[$i])
            { $count++ }
        }
    }
        
    return $firstHalf
}
 
function SecondHalf([array]$a)
{
    if ($a.Length -eq 1)
    {return}
    
    #write-host "inside SecondHalf()"
    $mid  = [Math]::Floor(($a.Length) / 2)
    $secondHalf = @()
    $mid..($a.Length - 1) | %{ $secondHalf += $a[$_] }
    
    #Find right inversions
    $count = 0
    0..(($secondHalf.Length) - 1) | %{
        for ($i = $_; $i -lt $secondHalf.Length; $i++)
        {
            if ($secondHalf[$_] -gt $secondHalf[$i])
            { $count++ }
        }
    }
    
    return $secondHalf
}
 
#Main
#$script:Count = 0
$script:RecursionCount = -1
#output list
$script:C = @()

#$A = gc "D:\MyData\Coursera\Algorithms_Design and Analysis_Part I\Week1\IntegerArray.txt"

[int]$len = $A.Length
Write-host -fore magenta "Calling recursive SourtAndCount"
$inv = SortAndCount $A $len
Write-Host -fore magenta "`nInversion = $($inv[0]), Sorted Array --> $($inv[1])"
