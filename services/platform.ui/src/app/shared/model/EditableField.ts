export interface EditableField {
    field: string;
    value: string | boolean;  // ✅ Accept both string and boolean values
    editable: boolean;
    isRestricted: boolean;
    originalValue: string | boolean;  // ✅ Update originalValue type as well
    inputType?: 'text' | 'boolean';  // ✅ Optional property to determine input type
  }