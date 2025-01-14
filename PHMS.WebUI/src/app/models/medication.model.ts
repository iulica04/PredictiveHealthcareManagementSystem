export interface Medication {
    id: string; 
    treatmentId: string; 
    name: string;
    type: MedicationType;
    ingredients: string;
    adverseEffects: string;
  }
  
  export enum MedicationType {
    Tablet,
    Capsule,
    Liquid,
    Injection,
    Inhaler,
    Topical,
    Suppository,
    Drops,
    Other
  }