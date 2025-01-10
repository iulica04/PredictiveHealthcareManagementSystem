import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NavbarComponent } from '../navbar/navbar.component';

@Component({
  selector: 'app-specializations',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './specializations.component.html',
  styleUrls: ['./specializations.component.css']
})
export class SpecializationsComponent {
  specializations: string[] = [
    'Cardiology', 'Neurology', 'Orthopedics', 'Pediatrics', 'Dermatology', 'Oncology', 'Gastroenterology', 
    'Urology', 'Psychiatry', 'Internal Medicine', 'Endocrinology', 'Hematology', 'Infectious Diseases', 
    'Geriatrics', 'Immunology', 'Radiology', 'Rheumatology', 'Rehabilitation Medicine', 'Sports Medicine', 
    'Intensive Care Medicine', 'Toxicology', 'Genetics', 'Nutrition', 'Allergy', 'Family Medicine', 
    'Plastic Surgery', 'General Surgery', 'Cardiovascular Surgery', 'Thoracic Surgery', 'Pediatric Surgery', 
    'Neurosurgery', 'Obstetrics and Gynecology', 'Maxillofacial Surgery', 'Vascular Surgery', 'Bariatric Surgery', 
    'Endoscopic Surgery', 'Orthopedic Surgery', 'Oncological Surgery', 'Spinal Surgery', 'Plastic and Reconstructive Surgery'
  ];
  selectedSpecialization: string = '';
  specializationDetails: string = '';

  constructor() { }

  ngOnInit(): void {
  }

  filterBySpecialization(event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedSpecialization = selectElement.value;
    this.specializationDetails = this.getSpecializationDetails(this.selectedSpecialization);
  }

  getSpecializationDetails(specialization: string): string {
    const details: { [key: string]: string } = {
      'Cardiology': 'Cardiology is the branch of medicine that deals with diseases and abnormalities of the heart. Common conditions include heart failure, coronary artery disease, and arrhythmias.',
      'Neurology': 'Neurology focuses on the diagnosis and treatment of disorders of the nervous system, including epilepsy, stroke, and multiple sclerosis.',
      'Orthopedics': 'Orthopedics is the branch of surgery concerned with conditions involving the musculoskeletal system, such as fractures, arthritis, and sports injuries.',
      'Pediatrics': 'Pediatrics involves the medical care of infants, children, and adolescents, addressing common childhood illnesses and developmental disorders.',
      'Dermatology': 'Dermatology deals with the skin, nails, hair, and their diseases, including eczema, psoriasis, and skin cancer.',
      'Oncology': 'Oncology focuses on the prevention, diagnosis, and treatment of cancer, including chemotherapy, radiation therapy, and immunotherapy.',
      'Gastroenterology': 'Gastroenterology is dedicated to the digestive system and its disorders, such as irritable bowel syndrome (IBS) and liver diseases.',
      'Urology': 'Urology focuses on diseases of the urinary tract and male reproductive organs, such as kidney stones, bladder infections, and prostate issues.',
      'Psychiatry': 'Psychiatry deals with mental health conditions, including depression, anxiety disorders, and schizophrenia.',
      'Internal Medicine': 'Internal medicine involves the diagnosis and treatment of internal organ diseases, such as hypertension, diabetes, and autoimmune diseases.',
      'Endocrinology': 'Endocrinology focuses on the endocrine system and hormonal disorders, such as diabetes, thyroid diseases, and osteoporosis.',
      'Hematology': 'Hematology deals with blood-related conditions, including anemia, leukemia, and clotting disorders.',
      'Infectious Diseases': 'This specialization addresses diseases caused by bacteria, viruses, fungi, and parasites, such as HIV/AIDS, tuberculosis, and COVID-19.',
      'Geriatrics': 'Geriatrics focuses on healthcare for elderly individuals, addressing aging-related conditions such as dementia and osteoporosis.',
      'Immunology': 'Immunology studies the immune system and treats immune-related disorders, such as allergies, asthma, and autoimmune diseases.',
      'Radiology': 'Radiology uses imaging technologies, such as X-rays, MRI, and CT scans, to diagnose and treat various medical conditions.',
      'Rheumatology': 'Rheumatology specializes in autoimmune and inflammatory diseases affecting joints and connective tissue, such as arthritis and lupus.',
      'Rehabilitation Medicine': 'This field focuses on restoring function and quality of life after injury or illness, including physical and occupational therapy.',
      'Sports Medicine': 'Sports medicine treats injuries and conditions related to sports and physical activity, such as sprains and ACL tears.',
      'Intensive Care Medicine': 'This specialization involves the care of critically ill patients, often in an ICU, addressing organ failure and severe infections.',
      'Toxicology': 'Toxicology deals with the effects of chemicals and toxins on the human body, including poisonings and drug overdoses.',
      'Genetics': 'Genetics focuses on hereditary diseases and conditions, such as cystic fibrosis and genetic counseling for inherited disorders.',
      'Nutrition': 'Nutrition involves the study of diet and its impact on health, including dietary therapy for obesity and eating disorders.',
      'Allergy': 'This field focuses on diagnosing and treating allergic conditions, such as hay fever, food allergies, and anaphylaxis.',
      'Family Medicine': 'Family medicine provides comprehensive healthcare for individuals and families across all ages and conditions.',
      'Plastic Surgery': 'Plastic surgery involves reconstructive and cosmetic procedures, such as breast reconstruction and facelifts.',
      'General Surgery': 'General surgery focuses on operations involving the abdomen, breast, and other general areas, such as hernia repair.',
      'Cardiovascular Surgery': 'This field involves surgical procedures on the heart and blood vessels, including bypass surgery and valve replacement.',
      'Thoracic Surgery': 'Thoracic surgery addresses conditions of the chest, including lung cancer and esophageal surgery.',
      'Pediatric Surgery': 'Pediatric surgery deals with surgical care for infants and children, including congenital abnormalities.',
      'Neurosurgery': 'Neurosurgery focuses on surgical treatment of disorders of the brain, spinal cord, and nervous system.',
      'Obstetrics and Gynecology': 'This field addresses women’s reproductive health, pregnancy, and childbirth.',
      'Maxillofacial Surgery': 'Maxillofacial surgery treats conditions of the face, jaw, and mouth, such as facial trauma and cleft palate.',
      'Vascular Surgery': 'Vascular surgery involves treating conditions of the vascular system, such as aneurysms and varicose veins.',
      'Bariatric Surgery': 'Bariatric surgery focuses on weight loss procedures, such as gastric bypass and sleeve gastrectomy.',
      'Endoscopic Surgery': 'Endoscopic surgery uses minimally invasive techniques to diagnose and treat conditions inside the body.',
      'Orthopedic Surgery': 'Orthopedic surgery treats musculoskeletal conditions, including joint replacements and spinal surgeries.',
      'Oncological Surgery': 'Oncological surgery involves removing tumors and cancerous tissue to treat cancer.',
      'Spinal Surgery': 'Spinal surgery addresses conditions affecting the spine, including herniated discs and scoliosis.',
      'Plastic and Reconstructive Surgery': 'This specialization focuses on restoring form and function, including burn reconstruction and complex trauma repair.'
    };
    return details[specialization] || 'Please select a specialization to see the details.';
  }
}
