# This file extracts and prints the VersionPrefix from a csproj file. 
# Must be called like this:   tclsh extract-vers.tcl xyz.csproj  

proc readLines { fileName error } {
    upvar $error err 
    if { [file exists $fileName] == 0 } {
	set err ERR_FILE_NOT_FOUND
	return 0
    }
    set f [open $fileName]
    set lineList [read $f]
    close $f
    set ll [split $lineList \n]
    set ll [lreplace $ll end end]
    return $ll
}

proc getValueFromKey {lines key default} {
	upvar $lines ll
	foreach line $ll {
		#puts "getValueFromIniLines 1.1 line=$line"
		if { [regexp "<$key>(.*)</$key>" $line r r1] !=0 } {
			#puts "getValueFromIniLines 1.2"
			return $r1
		}
	}
	return $default
}

set filename [lindex $argv 0]
#puts "LOOKING FOR VERS in $filename"
set err {}
set ll [readLines $filename err]
#puts LL=$ll
set vers [getValueFromKey ll {VersionPrefix} {0}]
puts $vers