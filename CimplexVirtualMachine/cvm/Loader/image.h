#include "..\Global\globaldef.h"

#ifndef _IMAGE_
#define _IMAGE_

typedef struct FileHeader
{
	DWORD TimeDateStamp;
	BYTE Characteristic;
	     /*
		     Characteristic describes the basic information of the file.

	 		 0x00: Object File
			 0x01: Executable File
			 0x02: Library File
			 0x03: Both a Executable and Library File
	      */
	ADDRESS EntryPointAddress;

	WORD MajorVersion;
	WORD MinorVersion;
	WORD InternelVersion;
	WORD Revision;

	ADDRESS ProgramName;
	ADDRESS DeveloperName;

	WORD LeastVMVersion;
	BYTE Subsystem;
	     /*
	         Subsystem describes how the program appears.

			 0x00: Command Line
			 0x01: GUI (Future Use)
			 0x02: Background Service (Future Use)
		  */
};

#endif