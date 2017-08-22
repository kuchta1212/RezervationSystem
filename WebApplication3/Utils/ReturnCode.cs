using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationSystem.Utils
{
    public enum ReturnCode
    {

        RELOAD_PAGE = 0,

        RESERVATION_SUCCESS = 1,

        RESERVATION_ERROR = 2,

        ERROR = 3,

        RELOAD_PAGE_WITH_PICKED_RESERVATIONS = 4,

        RELOAD_PAGE_TABLE_ALREADY_PICKED = 5,

        GROUP_RESERVATION_SUCCESS = 6,

        RESERVATION_SUCCESSFULLY_DELETED = 7
    }
}