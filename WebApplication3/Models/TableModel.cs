using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ReservationSystem.Models
{
    public class TableModel
    {
        public TableModel() { }

        public TableModel(int number)
        {
            this.Number = number;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int Number { get; private set; }



    }
}