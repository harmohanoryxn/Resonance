using System;
using System.Collections.Generic;
using Cloudmaster.WCS.Classes;

namespace Cloudmaster.WCS.IO
{
    public class FormRepository
    {
        public static Guid CleaningFormDefinitionGuid = new Guid("{BF3CC30B-7641-4F3B-91A3-2D3FEDA2BA9C}");

        public static Guid DailyFireChecklistFormDefinitionGuid = new Guid("{D89BFFB6-825E-4B20-86E7-C12335990216}");

        public static Guid InfectionControlAuditGuid = new Guid("{9A7D23E5-A2FB-4C42-AC3A-36729F046733}");

        public static FormDefinition GetCleaningFormDefinition()
        {
            FormDefinition formDefinition = new FormDefinition(CleaningFormDefinitionGuid) { Name = "Clean Room Checklist", EntityType = "Bedroom", Frequency = "Daily", Description = "Complete each time a room has been cleaned." };

            Section section = new Section(Guid.NewGuid ()) { Name = "Cleaning" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Room Clean", Description = "Room is cleaned and ready for use by patients.", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            return formDefinition;
        }

        public static FormDefinition GetDailyVisualFireSafetyChecklist()
        {
            FormDefinition formDefinition = new FormDefinition(DailyFireChecklistFormDefinitionGuid) { Name = "Visual Fire Safety Daily Checklist", EntityType = "Bedroom", Frequency = "Daily", Description = "Complete by 5pm each day" };

            Section section = new Section(Guid.NewGuid()) { Name = "Fire Safety" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Clear Escape Routes", Description = "Are all means of escape kept clear and free from obstruction?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Escape Maps", Description = "Are escape maps with evacuation routes clearly displayed?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Fire doors.", Description = "Fire doors are NOT wedged open?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Fire Extinguishers", Description = "Are fire Extinguishers in place and serviced? Are the pin and tag in place?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Fire Extinguishers Signs", Description = "Are the locations of fire extinguishers clearly signed?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Emergency Lighting", Description = "Are all emergency exit lights lit?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Break Glass Units", Description = "Are all Red break glass units intact i.e. panel not broken on glass?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Alarm Repeater Panel", Description = "Does the fire alarm repeater panel indicate normal operation?", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);
            
            return formDefinition;
        }

        public static FormDefinition GetInfectionControlAudit()
        {
            FormDefinition formDefinition = new FormDefinition(InfectionControlAuditGuid) { Name = "Infection Control Audit Tool", EntityType = "Bedroom", Frequency = "Monthly", Description = "" };

            Section section = new Section(Guid.NewGuid()) { Name = "General Environment & Patients Room" };

            // Hand Hygiene

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand Hygiene", Description = "Adequate facilities for hand hygiene are available in acordance with guidelines", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand Hygiene", Description = "Alcohol gel holders, towel & soap holders are clean with supply available.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand Hygiene", Description = "Gel, soap & hibiscrub/betidine scrub holders etc have appropriate plungers.", DefaultValue = "True", Result = "True" });

            // Spillage

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Free of splashes, soil, film, dust, fingerprints and spillage", Description = "Chair, Lockers Stools & Tables", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Free of splashes, soil, film, dust, fingerprints and spillage", Description = "Bed frames are clean and free from dust", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Free of splashes, soil, film, dust, fingerprints and spillage", Description = "Telephones and electronics", DefaultValue = "True", Result = "True" });
        
            // In good state of repair

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Lockers, Tables, Chairs", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Mattress, pillows and beds", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Chairs are covered with washable material", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Floors including edges and corners are free of dust and grit", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "All high and low surfaces are free from dust and cobwebs", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Curtains are free from stains dust and cobwebs", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Blinds are free from stains, dust and cobwebs", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "There is evidence of an effective pre-planned programme for curtain change", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Fans are clean and free from dust", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Air vents are clean and free from excess dust", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Patient call bells are clean and free from debris", DefaultValue = "True", Result = "True" });

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Ear phone pads are single use and changed between patients", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Reusable ear phones are cleaned between patients", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Patient audio visual systems are clean and free of dust", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Work station equipment in clinical areas are visible clean", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Cardiac arrest trolley is clean", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "No inappropiate items on the corridors", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Linen trolley on the corridor is covered", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment in good state of repair", Description = "Windows are closed", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            // Isolation Room

            section = new Section(Guid.NewGuid()) { Name = "Isolation Room" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Doors", Description = "Doors are closed", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Clean tidy trolley", Description = "Clean tidy trolley with no inappropiate items is set up outside isolation room", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Alcohol gel", Description = "There is alcohol gel outside the door", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Door sign", Description = "Door sign is in appropiate place, tidy and not mounted with tape", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Alcohol gel", Description = "Inside the patients room alcohol gel and hibiscrub is available as appropriate", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Yellow waste bin", Description = "The yellow waste bin is inside the room for contact and droplet isolation/transmission based precautions", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Protective isolation", Description = "For protective isolationprecautions the waste bin is outside the room.", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            // Clinical Room/Clean Store

            section = new Section(Guid.NewGuid()) { Name = "Clinical room/clean store" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Sterile Equipment", Description = "There is an identified area for the storage of clean and sterile equipment", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Sink", Description = "No sterile clean/non bottle items under sink", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Cleanliness", Description = "The area is clean and there is no inappropriate items are equipment", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand hygiene", Description = "Hand hygiene facilities are available in the clinical room/clean store", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Floor", Description = "Floor including edges and corners are free from dust", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Surfaces", Description = "All high and low surfaces are free from dust and cobwebs", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Shelves, bench tops", Description = "Shelves, bench tops, fridge and cupboards are clean inside and out, and are free of dust and spillage", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "No outdated lotions or drugs", Description = "No outdated lotions or drugs in use, in cupboards or in fridges", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Fridge Temperatues", Description = "Temperature are recorded on fridge daily and less than or equal to 5C", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Products", Description = "All products are stored above floor level", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Wall mounted signs", Description = "Wall mounted signs is washable/laminated and not mounted with tape", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Sharp boxes", Description = "Sharps boxes are available and assembled correctly", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Linen storage", Description = "Linen stored appropriately, covered in closed room or cupboard", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            // Bathroom/Washroom

            section = new Section(Guid.NewGuid()) { Name = "Bathroom/washroom" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Cleanliness", Description = "Bathrooms/washrooms are clean", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Inappropiate storage", Description = "There is no evidence of inappropiate storage of communal items e.g. creams, powder", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Equipment", Description = "Bathrooms are not used for equipment storage", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Cleanliness", Description = "Baths, sinks and accessories are clean", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Wall tiles", Description = "Wall tiles and wall fixtures are clean and free from mould", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Shower curtains", Description = "Shower curtains and bath mats are free from mould, clean and dry", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Weekly water running", Description = "There is evidence that baths, showers and sinks taken out of planned provision for running the water weekly", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Cleaning Materials", Description = "Appropriate cleaning materials are available for staff to clean the bath between use", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Floors", Description = "Floors including edges and corners are free of dust and grit.", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            // Toilet Staff

            section = new Section(Guid.NewGuid()) { Name = "Toilets Staff" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Cleanliness", Description = "The toilet hand wash sink, and surronding area is clean and free from extra items", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Floors", Description = "Floors including edges and corners are free of dust and grit", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand washing", Description = "Hand washing facilities are available, clean, including soap, alcohol gel and paper towels", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Sanitary waster", Description = "There is a facility for sanitary waste disposal", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Windows", Description = "Windows are closed", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);

            // Dirty Utility

            section = new Section(Guid.NewGuid()) { Name = "Dirty Utility" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Dirty utility", Description = "A dirty utility is available", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Defined areas", Description = "If clean items are in the sluice; the clean and dirty areas is clearly defined", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hand washing", Description = "Clean and dirty are seperate and no dirty items are stored in the clean area", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Nursing Staff", Description = "Nursing staff know how to clean used, dirty items", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Nursing Staff", Description = "Nursing staff know how to clean blood spills", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Accomidation Staff", Description = "Accomidation staff know how to clean used, dirty items", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Accomidation Staff", Description = "Accomidation staff know how to clean blood spills", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Decontamination sink", Description = "A seperate sink is available for decontamination of patient equipment", DefaultValue = "True", Result = "True" });

            formDefinition.Sections.Add(section);



            return formDefinition;
        }


        public static IList<FormDefinition> GetFormDefinitions()
        {
            List<FormDefinition> definitions = new List<FormDefinition>();

            /*
            FormDefinition slips = new FormDefinition(new Guid("{D89BFFB6-825E-4B20-86E7-C12335970216}")) { Name = "Monthly Floor and Passageways Checklist", EntityType = "Ward", Frequency = "Monthly", Description = "Please complete before the end of each month." };

            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Floors", Description = "Are floors and passageways free from slippery materials, loose objects and equipment?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Floors", Description = "Are floors maintained in good condition?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Materials", Description = "Are there materials of equipment on stairs or at the bottom of stairs/ under stairwell?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Materials", Description = "Are there materials or equipment on stairs or at the bottom of stairs / under stairwell?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Containers", Description = "Are there sufficient containers available for waste?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Electricity", Description = "Are there any trailing electric cables or wires on floors?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Electricity", Description = "Is there any trailing electric cables or wires on floors?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Storage", Description = "Is there sufficient filing and storage space?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Safety Awareness", Description = "Staff Safety awareness - Safety Consultation and Incident Reporting" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Safety Awareness", Description = "Are staff aware of Hospital Safety Consultation process? (Committees, safety representatives etc." });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Safety Awareness", Description = "Are staff aware of incident accident reporting procedures?" });
            slips.Checks.Add(new Check(Guid.NewGuid()) { Name = "Safety Awareness", Description = "Have all new staff been inducted in the departmetn (read health and safety policies, know how to use equipment etc)" });

            definitions.Add(slips);
             * */

            FormDefinition monthlyRoomChecklist = GetMonthlyMaintainanceAudit();

            definitions.Add(monthlyRoomChecklist);

            return definitions;
        }


        public static FormDefinition GetMonthlyMaintainanceAudit()
        {
            FormDefinition monthlyRoomChecklist = new FormDefinition(new Guid("{af877ff2-b0aa-4527-8cf0-9cd964249a07}")) { Name = "Monthly Bedroom Audit", EntityType = "Bedroom", Frequency = "Monthly", Description = "Please complete before the end of each month." };

            Section section = new Section(Guid.NewGuid ()) { Name = "Enviroment" };

            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Window Curtains", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Bin", Description = "Please ensure is in place is empty.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "T.V. Etc.", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Bedside Trolley", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Ski sheet on Bed", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Glass Shelf", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Toilets", Description = "Are toilets in good working order?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Toilet Roll Holder", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Toilet Brush", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Hygiene", Description = "Is the standard of hygiene satisfactory?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Shower Head", Description = "Is shower head clean?", DefaultValue = "True", Result = "True" });
            section.Checks.Add(new Check(Guid.NewGuid()) { Name = "Shower Curtain", Description = "Please ensure is in place and is not damaged.", DefaultValue = "True", Result = "True" });

            monthlyRoomChecklist.Sections.Add(section);

            return monthlyRoomChecklist;
        }
    }
}
