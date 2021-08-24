$append = $Args[0];
$baseDir = $Args[1];
if ($null -eq $baseDir) {
    $baseDir = "./"
}

$mainContent = Get-Content -Path "$($baseDir)Coosu.Storyboard.Storybrew.xml"

$xdoc = [System.Xml.Linq.XDocument]::Parse($mainContent)
$members = $xdoc.Element("doc").Element("members")

Select-Xml -Path "$($baseDir)Coosu.Shared.xml" -XPath '/doc/members/*' | ForEach-Object { $members.Add([System.Xml.Linq.XElement]::Parse($_.Node.OuterXml)) }
Select-Xml -Path "$($baseDir)Coosu.Storyboard.xml" -XPath '/doc/members/*' | ForEach-Object { $members.Add([System.Xml.Linq.XElement]::Parse($_.Node.OuterXml)) }
Select-Xml -Path "$($baseDir)Coosu.Storyboard.Extensions.xml" -XPath '/doc/members/*' | ForEach-Object { $members.Add([System.Xml.Linq.XElement]::Parse($_.Node.OuterXml)) }

$str = "<?xml version=""1.0""?>`n$($xdoc.ToString())";
Write-Output $str >"$($baseDir)Coosu.Storyboard.Storybrew-ILMerged$($append).xml"