#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class AccessControl
{
public:
	AccessControl(void* sdkContext);
	virtual ~AccessControl();


public:
	int getAccessSchedule(BS2_DEVICE_ID id, BS2Schedule& schedule);
	int setAccessSchedule(BS2_DEVICE_ID id, const BS2Schedule& schedule);
	int getAllAccessSchedule(BS2_DEVICE_ID id, std::vector<BS2Schedule>& schedules);
	int getAccessGroup(BS2_DEVICE_ID id, const BS2_ACCESS_GROUP_ID* groupIDs, uint32_t numOfIDs, std::vector<BS2AccessGroup>& groupList);


public:
	static void print(const BS2Schedule& schedule);
	static void print(const BS2DaySchedule& schedule);
	static void print(const BS2HolidaySchedule schedule);


private:
	void* context_;
};
