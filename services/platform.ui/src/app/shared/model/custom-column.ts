export interface CustomColumn {
    name: string;
    value: string;
    required?: boolean;
    position?: 'left' | 'right';
    width?: number;
    fixWidth?: boolean;
    default?: boolean;
  }
  