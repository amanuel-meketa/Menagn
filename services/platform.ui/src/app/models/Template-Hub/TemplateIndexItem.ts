export interface TemplateIndexItem {
author: any;
  key: string;
  name: string;
  category: string;
  description: string;
  stages: number;
  featured?: boolean; // optional flag for UI highlight
  version?: string;
}
