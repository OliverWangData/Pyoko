#ifndef COMMON_UTILITY_INCLUDE
#define COMMON_UTILITY_INCLUDE

// Repeats a statement x number of times. This is to unravel loops, which can cause issues with compilation.
// Need to manually update this if we want more tasks. More than 5 and it seems to start slowing down Unity shader compiler.
#define RepeatTask(task, count)	\
	if (count > 0) task;		\
	if (count > 1) task;		\
	if (count > 2) task;		\
	if (count > 3) task;		\
	if (count > 4) task;		\
	if (count > 5) task;
	
#endif // COMMON_UTILITY_INCLUDE