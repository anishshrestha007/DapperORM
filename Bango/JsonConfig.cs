using Bango.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango
{
    public class JsonConfig
    {
        public JsonSetting Relation = new JsonSetting("relation_settings.json", "mst_relation", "relation_id", "code");
        public JsonSetting ServiceType = new JsonSetting("service_state.json", "mst_service_state_hierarchy", "service_state_id", "code");
        public JsonSetting Org_Struc = new JsonSetting("org_structure.json", "mst_org_structure", "org_structure_id", "code");
        public JsonSetting Personnel = new JsonSetting("personnel_settings.json", "mst_personnel", "person_id", "code");
        public JsonSetting PositionHierarchy = new JsonSetting("position_hierarchy.json", "mst_position_hierarchy", "position_hierarchy_id", "code");
        public JsonSetting LocationHierarchy = new JsonSetting("location_hierarchy.json", "mst_location_hierarchy", "location_id", "code");
        public JsonSetting ActivityType = new JsonSetting("activity_type.json", "mst_activity", "activity_id", "code");
        public JsonSetting CountryType = new JsonSetting("country_settings.json", "mst_country", "country_id", "code");
        public JsonSetting JobStatus = new JsonSetting("job_status.json", "mst_job_status", "job_status_id", "code");
        public JsonSetting PromotionCalculation = new JsonSetting("promotion_calculation.json");
        public JsonSetting PromotionParams = new JsonSetting("promotion_params.json", "mst_promotion_params", "param_id", "code");
        public JsonSetting Division = new JsonSetting("division.json", "mst_division", "division_id", "code");
        public JsonSetting TrainingType = new JsonSetting("training_type.json", "mst_training_type", "training_type_id", "code");
        public JsonSetting UnitType = new JsonSetting("unit_type.json", "mst_unit_type", "unit_type_id", "code");

    }
}
