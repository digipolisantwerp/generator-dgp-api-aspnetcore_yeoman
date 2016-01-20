using AutoMapper;
using System;

namespace StarterKit
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            BusinessEntitiesToDataContracts();
            AgentToBusinessEntities();
			AgentToDataContracts();
        }

        private static void BusinessEntitiesToDataContracts()
        {
         
        }

        private static void AgentToBusinessEntities()
        {

		}

        private static void AgentToDataContracts()
        {
            
        }
    }
}