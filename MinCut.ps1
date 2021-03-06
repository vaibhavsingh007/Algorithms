
function GetMinCut($graph)
{
    if ($graph.Count -gt 2)
    {
        $randRowIndex = Get-Random -Minimum 0 -Maximum ($graph.Count-1)
        $randRow = $graph[$randRowIndex]
        $first = $randRow.Split(" ")[0]

        $second = $randRow.Split(" ", [StringSplitOptions]::RemoveEmptyEntries) | Get-Random
        while ($second -eq $first)
        {
            $second = $randRow.Split(" ", [StringSplitOptions]::RemoveEmptyEntries) | Get-Random
        }

        # set the new edges
        for ($i=0; $i -lt $graph.Count; $i++)
        {
            if ($graph[$i].StartsWith($second))
            {
                $secondVertexRow = $graph[$i]
                
                #set the second vertex row index to null
                $graph[$i] = $null
            }
            else
            {
                $graph[$i] = $graph[$i].Replace($second, $first)
            }
        }

        $randRow += " " + $secondVertexRow.Trim()
        $randRow = $randRow.Replace($second, $first)
        $randRow = $first + $($randRow.Substring(1)).Replace("$first ", "")
        
        # in case element $first is at the end. Note the space in replace
        $randRow = $first + $($randRow.Substring(1)).Replace("$first", "")
        $randRow = $randRow.Trim()

        $graph[$randRowIndex] = $randRow
        
        #construct new graph
        rv newGraph -ea SilentlyContinue
        $newGraph = $graph | ?{$_ -ne $null}
        
        #rv graph -ea SilentlyContinue
        #$graph = $newGraph
        #recursive call
        GetMinCut($newGraph)
    }
    if ($graph.Count -eq 2)
    {
        $firstCount = $($graph[0].Split(" ")).Count
        $secondCount = $($graph[1].Split(" ")).Count
        if ($firstCount -gt $secondCount)
        {
            if ($firstCount -ne $null)
            { $script:cut = $firstCount-1 }
        }
        else
        {
            if ($secondCount -ne $null)
            { $script:cut = $secondCount-1 }
        }
        
    }
    return
}

# Main
rv * -ea silentlycontinue
$minCut = 100
for ($j = 0; $j -lt 50; $j++)
{
    $initialGraph = gc D:\test.txt
    $script:cut = 0
    
    GetMinCut($initialGraph)
    if (($script:cut -lt $minCut) -and ($script:cut -ne 0))
    {
        $minCut = $script:cut
    }
    $minCut
}

Write-Host "Min cut found after 50 times: $minCut"